/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

See SampleFramework license.txt for license terms.  Unless required by applicable law
or agreed to in writing, the sample code is provided �AS IS� WITHOUT WARRANTIES OR
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific
language governing permissions and limitations under the license.

************************************************************************************/

using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using UnityEngine.Serialization;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace OVRTouchSample
{
    public class Hand : MonoBehaviour
    {
        public GameObject avatar;

        public const string AnimLayerNamePoint = "Point Layer";
        public const string AnimLayerNameThumb = "Thumb Layer";
        public const string AnimParamNameFlex = "Flex";
        public const string AnimParamNamePose = "Pose";
        public const float ThreshCollisionFlex = 0.9f;

        public const float InputRateChange = 20.0f;

        public const float ColliderScaleMIN = 0.01f;
        public const float ColliderScaleMAX = 1.0f;
        public const float ColliderScalePerSecond = 1.0f;

        public const float TriggerDebounceTime = 0.05f;
        public const float ThumbDebounceTime = 0.15f;

        
        [FormerlySerializedAs("m_controller")] [SerializeField]
        private OVRInput.Controller mController = OVRInput.Controller.None;
        [FormerlySerializedAs("m_animator")] [SerializeField]
        private Animator mAnimator = null;
        [FormerlySerializedAs("m_defaultGrabPose")] [SerializeField]
        private HandPose mDefaultGrabPose = null;

        private Collider[] _mColliders = null;
        private bool _mCollisionEnabled = true;
        public XRDirectInteractor mGrabber;
        private bool _isLocalHand = false;

        List<Renderer> _mShowAfterInputFocusAcquired;

        private int _mAnimLayerIndexThumb = -1;
        private int _mAnimLayerIndexPoint = -1;
        private int _mAnimParamIndexFlex = -1;
        private int _mAnimParamIndexPose = -1;

        private bool _mIsPointing = false;
        private bool _mIsGivingThumbsUp = false;
        private float _mPointBlend = 0.0f;
        private float _mThumbsUpBlend = 0.0f;

        private bool _mPointLock;
        private float _mPointLockBegin;
        private const float PointLockAnimationDuration = 0.1f;
        private float _mPointLockEnd => _mPointLockBegin + PointLockAnimationDuration;

        private bool _mRestoreOnInputAcquired = false;

        private ChangeHandGesture _changeHandGesture;
        public bool leftHand;

        private void Start()
        {
            if(avatar.GetComponent<PlayerAvatar>().isLocal){
                _isLocalHand = true;
            }

            if (_isLocalHand)
            {
                HandReference handRef = GameObject.Find("HandReference").GetComponent<HandReference>();
                mGrabber = leftHand
                    ? handRef.leftHand.GetComponent<XRDirectInteractor>()
                    : handRef.rightHand.GetComponent<XRDirectInteractor>();
            }

            _changeHandGesture = GetComponent<ChangeHandGesture>();

            _mShowAfterInputFocusAcquired = new List<Renderer>();

            // Collision starts disabled. We'll enable it for certain cases such as making a fist.
            _mColliders = this.GetComponentsInChildren<Collider>().Where(childCollider => !childCollider.isTrigger).ToArray();
            CollisionEnable(false);

            // Get animator layer indices by name, for later use switching between hand visuals
            _mAnimLayerIndexPoint = mAnimator.GetLayerIndex(AnimLayerNamePoint);
            _mAnimLayerIndexThumb = mAnimator.GetLayerIndex(AnimLayerNameThumb);
            _mAnimParamIndexFlex = Animator.StringToHash(AnimParamNameFlex);
            _mAnimParamIndexPose = Animator.StringToHash(AnimParamNamePose);

            OVRManager.InputFocusAcquired += OnInputFocusAcquired;
            OVRManager.InputFocusLost += OnInputFocusLost;
            
            #if UNITY_EDITOR
            OVRPlugin.SendEvent("custom_hand", (SceneManager.GetActiveScene().name == "CustomHands").ToString(), "sample_framework");
            #endif
        }

        private int _pointLockCounter;
        public void SetPointLock(bool isPointing)
        {
            _pointLockCounter += isPointing ? 1 : -1;

            if (isPointing == _mPointLock) return;

            _mPointLock = _pointLockCounter != 0;
            if((isPointing && _pointLockCounter == 1) || (!isPointing && _pointLockCounter == 0))
                _mPointLockBegin = Time.time;
        }

        // Returns a value used for lerping to the point lock animation
        private float PointLockLerpValue()
        {
            float value = (Time.time - _mPointLockBegin) / (_mPointLockEnd - _mPointLockBegin);

            if(_mPointLock)
                return value;

            return 1 - value;
        }

        private void OnDestroy()
        {
            OVRManager.InputFocusAcquired -= OnInputFocusAcquired;
            OVRManager.InputFocusLost -= OnInputFocusLost;
        }

        private void Update()
        {
            if(!_isLocalHand){
                return;
            }

            UpdateCapTouchStates();

            _mPointBlend = InputValueRateChange(_mIsPointing, _mPointBlend);
            _mThumbsUpBlend = InputValueRateChange(_mIsGivingThumbsUp, _mThumbsUpBlend);

            UpdateAnimStates();
        }

        // Just checking the state of the index and thumb cap touch sensors, but with a little bit of
        // debouncing.
        private void UpdateCapTouchStates()
        {
            _mIsPointing = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, mController);
            _mIsGivingThumbsUp = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, mController);
        }

        private void LateUpdate()
        {
            // Hand's collision grows over a short amount of time on enable, rather than snapping to on, to help somewhat with interpenetration issues.
            if (_mCollisionEnabled && _mCollisionScaleCurrent + Mathf.Epsilon < ColliderScaleMAX)
            {
                _mCollisionScaleCurrent = Mathf.Min(ColliderScaleMAX, _mCollisionScaleCurrent + Time.deltaTime * ColliderScalePerSecond);
                for (int i = 0; i < _mColliders.Length; ++i)
                {
                    Collider collider = _mColliders[i];
                    collider.transform.localScale = new Vector3(_mCollisionScaleCurrent, _mCollisionScaleCurrent, _mCollisionScaleCurrent);
                }
            }
        }

        // Simple Dash support. Just hide the hands.
        private void OnInputFocusLost()
        {
            if (gameObject.activeInHierarchy)
            {
                _mShowAfterInputFocusAcquired.Clear();
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; ++i)
                {
                    if (renderers[i].enabled)
                    {
                        renderers[i].enabled = false;
                        _mShowAfterInputFocusAcquired.Add(renderers[i]);
                    }
                }

                CollisionEnable(false);

                _mRestoreOnInputAcquired = true;
            }
        }

        private void OnInputFocusAcquired()
        {
            if (_mRestoreOnInputAcquired)
            {
                for (int i = 0; i < _mShowAfterInputFocusAcquired.Count; ++i)
                {
                    if (_mShowAfterInputFocusAcquired[i])
                    {
                        _mShowAfterInputFocusAcquired[i].enabled = true;
                    }
                }
                _mShowAfterInputFocusAcquired.Clear();

                // Update function will update this flag appropriately. Do not set it to a potentially incorrect value here.
                //CollisionEnable(true);

                _mRestoreOnInputAcquired = false;
            }
        }

        private float InputValueRateChange(bool isDown, float value)
        {
            float rateDelta = Time.deltaTime * InputRateChange;
            float sign = isDown ? 1.0f : -1.0f;
            return Mathf.Clamp01(value + rateDelta * sign);
        }

        private void UpdateAnimStates()
        {
            // bool grabbing = mGrabber.grabbedObject != null;
            bool grabbing = mGrabber.selectTarget != null;

            // Pose
            //m_animator.SetInteger(m_animParamIndexPose, (int)handPoseId);
            _changeHandGesture.SetHandPoseId((int)HandPoseId.Default);

            // Flex
            // blend between open hand and fully closed fist
            float flex = grabbing ? 1.0f : OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, mController);
            //m_animator.SetFloat(m_animParamIndexFlex, flex);

            float pointLockLerpValue = PointLockLerpValue();

            flex = Mathf.Lerp(flex, 1f, pointLockLerpValue);
            _changeHandGesture.SetFlex(flex);

            // Point
            bool canPoint = !grabbing;
            float point = canPoint ? _mPointBlend : 0.0f;
            point = Mathf.Lerp(point, grabbing ? 0f : 1f, pointLockLerpValue);
            //m_animator.SetLayerWeight(m_animLayerIndexPoint, point);
            _changeHandGesture.SetPoint(point);

            // Thumbs up
            bool canThumbsUp = !grabbing;
            float thumbsUp = canThumbsUp ? _mThumbsUpBlend : 0.0f;
            thumbsUp = Mathf.Lerp(thumbsUp, 0f, pointLockLerpValue);
            //m_animator.SetLayerWeight(m_animLayerIndexThumb, thumbsUp);
            _changeHandGesture.SetThumbsUp(thumbsUp);

            // float pinch = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, mController);
            //m_animator.SetFloat("Pinch", pinch);
            // _changeHandGesture.SetPinch(pinch);
        }

        private float _mCollisionScaleCurrent = 0.0f;

        private void CollisionEnable(bool enabled)
        {
            if (_mCollisionEnabled == enabled)
            {
                return;
            }
            _mCollisionEnabled = enabled;

            if (enabled)
            {
                _mCollisionScaleCurrent = ColliderScaleMIN;
                for (int i = 0; i < _mColliders.Length; ++i)
                {
                    Collider collider = _mColliders[i];
                    collider.transform.localScale = new Vector3(ColliderScaleMIN, ColliderScaleMIN, ColliderScaleMIN);
                    collider.enabled = true;
                }
            }
            else
            {
                _mCollisionScaleCurrent = ColliderScaleMAX;
                for (int i = 0; i < _mColliders.Length; ++i)
                {
                    Collider collider = _mColliders[i];
                    collider.enabled = false;
                    collider.transform.localScale = new Vector3(ColliderScaleMIN, ColliderScaleMIN, ColliderScaleMIN);
                }
            }
        }
    }
}

public enum HandPoseId
    {
        Default,
        Generic,
        PingPongBall,
        Controller
    }

	// Stores pose-specific data such as the animation id and allowing gestures.
    public class HandPose : MonoBehaviour
    {
        [SerializeField]
        private bool m_allowPointing = false;
        [SerializeField]
        private bool m_allowThumbsUp = false;
        [SerializeField]
        private HandPoseId m_poseId = HandPoseId.Default;

        public bool AllowPointing
        {
            get { return m_allowPointing; }
        }

        public bool AllowThumbsUp
        {
            get { return m_allowThumbsUp; }
        }

        public HandPoseId PoseId
        {
            get { return m_poseId; }
        }
    }
