using System;
using UnityEngine;

[Serializable]
public class StretchUIAnimationProfile : UIAnimationProfile{
    [HideInInspector]
    public Transform AnimatedTransform;
    public Vector2 GoalScale;

    public override Type GetRequiredReferenceType(){
        return typeof(Transform);
    }

    public override bool IsUIReferenced(){
    return AnimatedTransform != null;
    }

    public override void ReferenceUI<T>(T reference){
    if (reference is Transform transformReference && transformReference != null){
            AnimatedTransform = transformReference;
        }
    }

    public override UIAnimationType GetAnimationType() => UIAnimationType.Stretch;
}