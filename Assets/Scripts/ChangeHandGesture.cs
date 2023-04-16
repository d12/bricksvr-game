using UnityEngine;

public class ChangeHandGesture : MonoBehaviour
{
    public GameObject handAnimatorObject;
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

    public void SetHandPoseId(int value) {
        if(animator == null) return;
        animator.SetInteger(animParamIndexPose, value);
    }

    public void SetFlex(float value) {
        if(animator == null) return;
        animator.SetFloat(animParamIndexFlex, value);
    }

    public void SetPoint(float value) {
        if(animator == null) return;
        animator.SetLayerWeight(animLayerIndexPoint, value);
    }

    public void SetThumbsUp(float value) {
        if(animator == null) return;
        animator.SetLayerWeight(animLayerIndexThumb, value);
    }

    public void SetPinch(float value) {
        if(animator == null) return;
        animator.SetFloat(animParamPinch, value);
    }
}
