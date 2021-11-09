using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class ChangeHandGesture : RealtimeComponent
{
    public GameObject handAnimatorObject;
    private HandGestureModel _model;
    private Animator animator;

    private int animLayerIndexThumb;
    private int animLayerIndexPoint;
    private int animParamIndexFlex;
    private int animParamIndexPose;
    private string animParamPinch = "Pinch";

    void Start()
    {
        animator = handAnimatorObject.GetComponent<Animator>();

        animLayerIndexPoint = animator.GetLayerIndex("Point Layer");
        animLayerIndexThumb = animator.GetLayerIndex("Thumb Layer");
        animParamIndexFlex = Animator.StringToHash("Flex");
        animParamIndexPose = Animator.StringToHash("Pose");
    }

    private HandGestureModel model {
        set {
            if (_model != null) {
                // Unregister from events
                _model.handPoseIdDidChange -= HandPoseIdDidChange;
                _model.flexDidChange -= FlexDidChange;
                _model.pointDidChange -= PointDidChange;
                _model.thumbsUpDidChange -= ThumbsUpDidChange;
                _model.pinchDidChange -= PinchDidChange;
            }

            // Store the model
            _model = value;

            if (_model != null) {
                UpdateHandAnimator();

                // Register for events so we'll know if the hand gesture changes later
                _model.handPoseIdDidChange += HandPoseIdDidChange;
                _model.flexDidChange += FlexDidChange;
                _model.pointDidChange += PointDidChange;
                _model.thumbsUpDidChange += ThumbsUpDidChange;
                _model.pinchDidChange += PinchDidChange;
            }
        }
    }

    private void HandPoseIdDidChange(HandGestureModel model, int value) {
        if(animator == null) return;
        animator.SetInteger(animParamIndexPose, value);
    }

    private void FlexDidChange(HandGestureModel model, float value) {
        if(animator == null) return;
        animator.SetFloat(animParamIndexFlex, value);
    }

    private void PointDidChange(HandGestureModel model, float value) {
        if(animator == null) return;
        animator.SetLayerWeight(animLayerIndexPoint, value);
    }

    private void ThumbsUpDidChange(HandGestureModel model, float value) {
        if(animator == null) return;
        animator.SetLayerWeight(animLayerIndexThumb, value);
    }

    private void PinchDidChange(HandGestureModel model, float value) {
        if(animator == null) return;
        animator.SetFloat(animParamPinch, value);
    }

    private void UpdateHandAnimator() {
        HandPoseIdDidChange(_model, _model.handPoseId);
        FlexDidChange(_model, _model.flex);
        PointDidChange(_model, _model.point);
        ThumbsUpDidChange(_model, _model.thumbsUp);
        PinchDidChange(_model, _model.pinch);
    }

    public void SetHandPoseId(int value) {
        _model.handPoseId = value;
    }

    public void SetFlex(float value) {
        _model.flex = value;
    }

    public void SetPoint(float value) {
        _model.point = value;
    }

    public void SetThumbsUp(float value) {
        _model.thumbsUp = value;
    }

    public void SetPinch(float value) {
        _model.pinch = value;
    }
}
