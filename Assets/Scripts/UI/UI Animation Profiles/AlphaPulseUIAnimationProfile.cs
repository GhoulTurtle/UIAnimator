using System;
using UnityEngine;

[Serializable]
public class AlphaPulseUIAnimationProfile : UIAnimationProfile{
	[HideInInspector]
	public CanvasGroup CanvasGroup;
	public float AlphaStart;
	public float AlphaEnd;
	public float AnimationSpeed;

    public override Type GetRequiredReferenceType(){
        return typeof(CanvasGroup);
    }

    public override bool IsUIReferenced(){
        return CanvasGroup != null;
    }

    public override void ReferenceUI<T>(T reference){
		if (reference is CanvasGroup canvasGroupReference && canvasGroupReference != null){
            CanvasGroup = canvasGroupReference;
        }
    }
    
    public override UIAnimationType GetAnimationType() => UIAnimationType.AlphaPulse;
}