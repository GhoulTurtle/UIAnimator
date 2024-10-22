using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class UIAnimationProfile : ICloneable{
	[HideInInspector]
	public MonoBehaviour AnimationRunner;
	public IEnumerator AnimationCoroutine;
	public float AnimationDuration;
	public bool DeactivateAfterAnimation;
	public bool ScaledDeltaTime;
	public bool QueuedAnimation;
	public Action OnAnimationFinishedCallback;

	protected bool startedAnimation = false;

	public virtual void ReferenceAnimation(MonoBehaviour animationRunner, IEnumerator animationCoroutine){
		AnimationRunner = animationRunner;
		AnimationCoroutine = animationCoroutine;
	}

	public virtual void StartAnimation(){
		if(AnimationRunner == null || AnimationCoroutine == null) return;

		if(startedAnimation){
			StopAnimation();
		}

		AnimationRunner.StartCoroutine(AnimationCoroutine);
		startedAnimation = true;
	}

	public virtual void StopAnimation(){
		if(AnimationRunner == null || AnimationCoroutine == null) return;
		
		AnimationRunner.StopCoroutine(AnimationCoroutine);
		AnimationCoroutine = null;
		startedAnimation = false;
	}

	public bool IsAnimationPlaying(){
		return startedAnimation;
	}

	public abstract UIAnimationType GetAnimationType();
	public abstract bool IsUIReferenced();
	public abstract Type GetRequiredReferenceType();
	public abstract void ReferenceUI<T>(T reference) where T : class;

    public object Clone(){
		var clonedProfile = (UIAnimationProfile)MemberwiseClone();

        return clonedProfile;
    }
}
