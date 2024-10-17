using System;
using UnityEngine;

[Serializable]
public class CosUIAnimationProfile : UIAnimationProfile{    
	[HideInInspector]
	public Transform AnimatedTransform;
	public float XOrigin;
	public float AnimationSpeed;
	public float AnimationDistance;

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

    public override UIAnimationType GetAnimationType() => UIAnimationType.Cos;
}