using System;
using UnityEngine;

[Serializable]
public class SinUIAnimationProfile : UIAnimationProfile{
	[HideInInspector]
	public Transform AnimatedTransform;
	public float YOrigin;
	public float AnimationSpeed;
	public float AnimationDistance;

    public override UIAnimationType GetAnimationType() => UIAnimationType.Sin;
    
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
}
