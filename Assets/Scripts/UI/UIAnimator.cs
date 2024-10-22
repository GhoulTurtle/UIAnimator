using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIAnimator{
    private const float LERP_SNAP_DISTANCE = 0.01f;

    private static Dictionary<MonoBehaviour, List<UIAnimationProfile>> animationDictionary = new Dictionary<MonoBehaviour, List<UIAnimationProfile>>();

    public static void ResetAnimationDictionary(){
        animationDictionary.Clear();
    }

    public static bool IsAnimationOfTypePlaying(MonoBehaviour animationRunner, UIAnimationType animationType){
        if(!animationDictionary.ContainsKey(animationRunner)) return false;

        List<UIAnimationProfile> animationProfiles = animationDictionary[animationRunner];
    
        for (int i = 0; i < animationProfiles.Count; i++){
            if(animationProfiles[i].GetAnimationType() == animationType){
                return true;
            }
        }

        return false;
    }

    public static UIAnimationProfile StartUIAnimation<T>(MonoBehaviour animationRunner, UIAnimationSO animationSO, T reference, Action OnAnimationFinishedCallback) where T : class{
        //Guard against invalid UI references
        UIAnimationProfile templateAnimationProfile = animationSO.GetTemplateAnimationProfile();

        if(templateAnimationProfile.GetRequiredReferenceType() != typeof(T)){
            Debug.LogWarning("Invalid UI animation reference. Aborting UI animation.");
            return null;
        }

        //Clone the template profile to a new instance of the profile
        UIAnimationProfile instanceAnimationProfile = (UIAnimationProfile)templateAnimationProfile.Clone();

        //Reference the UI
        instanceAnimationProfile.ReferenceUI(reference);

        if(!instanceAnimationProfile.IsUIReferenced()){
            //Invalid reference abort UI animation
            Debug.LogWarning("Invalid UI animation reference. Aborting UI animation.");
            return null;
        }

        if(OnAnimationFinishedCallback != null){
            instanceAnimationProfile.OnAnimationFinishedCallback = OnAnimationFinishedCallback;
        }

        //Set the correct animation coroutine on the profile
        switch (instanceAnimationProfile.GetAnimationType()){
            case UIAnimationType.Stretch:
                instanceAnimationProfile.ReferenceAnimation(animationRunner, StretchAnimationCoroutine(instanceAnimationProfile));
                break;
            case UIAnimationType.Sin:
                instanceAnimationProfile.ReferenceAnimation(animationRunner, SinAnimationCoroutine(instanceAnimationProfile));
                break;
            case UIAnimationType.Cos:
                instanceAnimationProfile.ReferenceAnimation(animationRunner, CosAnimationCoroutine(instanceAnimationProfile));
                break;
            case UIAnimationType.Lerp:
                instanceAnimationProfile.ReferenceAnimation(animationRunner, LerpingAnimationCoroutine(instanceAnimationProfile));
                break;
            case UIAnimationType.AlphaFade:
                instanceAnimationProfile.ReferenceAnimation(animationRunner, CanvasGroupAlphaFadeCoroutine(instanceAnimationProfile));
                break;
            case UIAnimationType.AlphaPulse:
                instanceAnimationProfile.ReferenceAnimation(animationRunner, CanvasGroupAlphaPulseCoroutine(instanceAnimationProfile));
                break;
        }

        //Add or Create a new entry in the animation dictionary
        List<UIAnimationProfile> animationProfiles;

        if(animationDictionary.ContainsKey(animationRunner)){
            animationProfiles = animationDictionary[animationRunner];

            animationProfiles.Add(instanceAnimationProfile);
        }
        else{
            animationProfiles = new List<UIAnimationProfile>{
                instanceAnimationProfile
            };

            animationDictionary.Add(animationRunner, animationProfiles);
        }

        //If the animation is queued then check if the queued animation is the first queued animation and if it is then play it 
        if(instanceAnimationProfile.QueuedAnimation){
            //Attempt to play the first queued animation
            AttemptPlayFirstQueuedAnimation(animationProfiles);
        }
        else{
            instanceAnimationProfile.StartAnimation();
        }

        return instanceAnimationProfile;
    }

    public static void StopUIAnimationOnRunner(MonoBehaviour animationRunner){ 
        if(animationRunner == null || !animationDictionary.ContainsKey(animationRunner)) return;

        List<UIAnimationProfile> animationProfiles = animationDictionary[animationRunner];

        for (int i = 0; i < animationProfiles.Count; i++){
            animationProfiles[i].StopAnimation();
        }

        if(animationDictionary.ContainsKey(animationRunner)){
            animationDictionary.Remove(animationRunner);
        }
    }

    public static void StopUIAnimationOnRunner(MonoBehaviour animationRunner, UIAnimationProfile animationProfile){
        if(animationRunner == null || !animationDictionary.ContainsKey(animationRunner)) return;

        List<UIAnimationProfile> animationProfiles = animationDictionary[animationRunner];

        if(!animationProfiles.Contains(animationProfile)) return;

        for (int i = 0; i < animationProfiles.Count; i++){
            if(animationProfiles[i] == animationProfile){
                animationProfiles[i].StopAnimation();
                animationProfiles.RemoveAt(i);
                break;
            }
        }

        AttemptPlayFirstQueuedAnimation(animationProfiles);
    }

    public static void StopUIAnimationOnRunner(MonoBehaviour animationRunner, UIAnimationType animationType){
        if(animationRunner == null || !animationDictionary.ContainsKey(animationRunner)) return;

        List<UIAnimationProfile> animationProfiles = animationDictionary[animationRunner];
    
        for (int i = 0; i < animationProfiles.Count; i++){
            if(animationProfiles[i].GetAnimationType() == animationType){
                animationProfiles[i].StopAnimation();
                animationProfiles.RemoveAt(i);
            }
        }

        AttemptPlayFirstQueuedAnimation(animationProfiles);
    }

    public static void UIAnimationFinished(UIAnimationProfile animationProfile, GameObject animatedGameobject){
        if(animationProfile.OnAnimationFinishedCallback != null){
            animationProfile.OnAnimationFinishedCallback?.Invoke();
        }

        if(animationProfile.DeactivateAfterAnimation && animatedGameobject != null){
            animatedGameobject.SetActive(false);
        }
        
        StopUIAnimationOnRunner(animationProfile.AnimationRunner, animationProfile);

        if(!animationDictionary.ContainsKey(animationProfile.AnimationRunner) && animationDictionary[animationProfile.AnimationRunner].Count > 0) return;

        //Attempt to play the next queued animation
        List<UIAnimationProfile> animationProfiles = animationDictionary[animationProfile.AnimationRunner];

        AttemptPlayFirstQueuedAnimation(animationProfiles);
    }

    private static void AttemptPlayFirstQueuedAnimation(List<UIAnimationProfile> animationProfiles){
        if(animationProfiles == null || animationProfiles.Count == 0) return;
        
        for (int i = 0; i < animationProfiles.Count; i++){
            if(!animationProfiles[i].QueuedAnimation) continue;

            if(animationProfiles[i].IsAnimationPlaying()) return;

            animationProfiles[i].StartAnimation();
        }
    }

    // A stretch animation that lerps from the current object scale to the goal scale over the animation duration.
    public static IEnumerator StretchAnimationCoroutine(UIAnimationProfile animationProfile){
        StretchUIAnimationProfile stretchUIAnimationProfile = (StretchUIAnimationProfile)animationProfile;

        var _goalScale = new Vector3(stretchUIAnimationProfile.GoalScale.x, stretchUIAnimationProfile.GoalScale.y, 1);

        float current = 0;

        Transform transformToAnimate = stretchUIAnimationProfile.AnimatedTransform;

        while(Vector3.Distance(transformToAnimate.localScale, _goalScale) > LERP_SNAP_DISTANCE){
            transformToAnimate.localScale = Vector3.Lerp(transformToAnimate.localScale, _goalScale, current / stretchUIAnimationProfile.AnimationDuration);
            if(stretchUIAnimationProfile.ScaledDeltaTime){
                current += Time.deltaTime;
            }
            else{
                current += Time.unscaledDeltaTime;
            }
            yield return null;
        }

        transformToAnimate.localScale = _goalScale;

        UIAnimationFinished(stretchUIAnimationProfile, transformToAnimate.gameObject);
    }

    // A sin animation that animates a transform local Y position following a sin wave. Can adjust the sin wave with the animationSpeed and animationDistance. 
    public static IEnumerator SinAnimationCoroutine(UIAnimationProfile animationProfile){
        SinUIAnimationProfile sinUIAnimationProfile = (SinUIAnimationProfile)animationProfile;
        Transform transformToAnimate = sinUIAnimationProfile.AnimatedTransform;

        while(sinUIAnimationProfile.AnimationDuration == -1f){
            transformToAnimate.localPosition = new Vector3(transformToAnimate.localPosition.x, SinAmount(sinUIAnimationProfile.YOrigin, sinUIAnimationProfile.AnimationSpeed, sinUIAnimationProfile.AnimationDistance, sinUIAnimationProfile.ScaledDeltaTime), transformToAnimate.localPosition.z);
            yield return null;
        }

        float animationTimer = sinUIAnimationProfile.AnimationDuration;
        while(animationTimer >= 0f){
            if(sinUIAnimationProfile.ScaledDeltaTime){
                animationTimer -= Time.deltaTime;
            }
            else{
                animationTimer -= Time.unscaledDeltaTime;
            }
            transformToAnimate.localPosition = new Vector3(transformToAnimate.localPosition.x, SinAmount(sinUIAnimationProfile.AnimationDuration, sinUIAnimationProfile.AnimationSpeed, sinUIAnimationProfile.AnimationDistance, sinUIAnimationProfile.ScaledDeltaTime), transformToAnimate.localPosition.z);
            yield return null;
        }

        transformToAnimate.localPosition = new Vector3(transformToAnimate.localPosition.x, sinUIAnimationProfile.YOrigin, transformToAnimate.localPosition.z);

        UIAnimationFinished(sinUIAnimationProfile, transformToAnimate.gameObject);
    }

    private static float SinAmount(float yOrigin, float sinSpread, float sinIntensity, bool scaledTime = true){
        if(scaledTime){
            return yOrigin + Mathf.Sin(Time.time * sinSpread) * sinIntensity;
        }
        
        return yOrigin + Mathf.Sin(Time.unscaledTime * sinSpread) * sinIntensity;
    }

    // A cos animation that animates a transform local X position following a cos wave. Can adjust the cos wave with the animationSpeed and animationDistance. 
    public static IEnumerator CosAnimationCoroutine(UIAnimationProfile animationProfile){
        CosUIAnimationProfile cosUIAnimationProfile = (CosUIAnimationProfile)animationProfile;
        Transform transformToAnimate = cosUIAnimationProfile.AnimatedTransform;
        
        while(cosUIAnimationProfile.AnimationDuration == -1f){
            transformToAnimate.localPosition = new Vector3(CosAmount(cosUIAnimationProfile.XOrigin, cosUIAnimationProfile.AnimationSpeed, cosUIAnimationProfile.AnimationDistance, cosUIAnimationProfile.ScaledDeltaTime), transformToAnimate.localPosition.y, transformToAnimate.localPosition.z);
            yield return null;
        }

        float animationTimer = cosUIAnimationProfile.AnimationDuration;
        while(animationTimer >= 0f){
            if(cosUIAnimationProfile.ScaledDeltaTime){
                animationTimer -= Time.deltaTime;
            }
            else{
                animationTimer -= Time.unscaledDeltaTime;
            }
            transformToAnimate.localPosition = new Vector3(CosAmount(cosUIAnimationProfile.XOrigin, cosUIAnimationProfile.AnimationSpeed, cosUIAnimationProfile.AnimationDistance, cosUIAnimationProfile.ScaledDeltaTime), transformToAnimate.localPosition.y, transformToAnimate.localPosition.z);            
            yield return null;
        }

        transformToAnimate.localPosition = new Vector3(cosUIAnimationProfile.XOrigin, transformToAnimate.localPosition.y, transformToAnimate.localPosition.z);    
        
        UIAnimationFinished(cosUIAnimationProfile, transformToAnimate.gameObject);
    }

    private static float CosAmount(float xOrigin, float cosSpread, float cosIntensity, bool scaledTime = true){
        if(scaledTime){
            return xOrigin + Mathf.Cos(Time.time * cosSpread) * cosIntensity;
        }

        return xOrigin + Mathf.Cos(Time.unscaledTime * cosSpread) * cosIntensity;
    }

    // A lerp animation that lerps from the current object position to the goal position over the animation duration.
    public static IEnumerator LerpingAnimationCoroutine(UIAnimationProfile animationProfile){
        LerpUIAnimationProfile lerpUIAnimationProfile = (LerpUIAnimationProfile)animationProfile;
        Transform transformToAnimate = lerpUIAnimationProfile.AnimatedTransform;

        var _goalPosition = new Vector3(lerpUIAnimationProfile.GoalPosition.x, lerpUIAnimationProfile.GoalPosition.y, 1);
        
        float current = 0;

        while(Vector3.Distance(transformToAnimate.localPosition, _goalPosition) > LERP_SNAP_DISTANCE){
            transformToAnimate.localPosition = Vector3.Lerp(transformToAnimate.localPosition, _goalPosition, current / lerpUIAnimationProfile.AnimationDuration);
            if(lerpUIAnimationProfile.ScaledDeltaTime){
                current += Time.deltaTime;
            }
            else{
                current += Time.unscaledDeltaTime;
            }
            yield return null;
        }

        transformToAnimate.localPosition = _goalPosition;

        UIAnimationFinished(lerpUIAnimationProfile, transformToAnimate.gameObject);
    }
   
    /// A lerp animation that lerps a canvas group's alpha value.
    public static IEnumerator CanvasGroupAlphaFadeCoroutine(UIAnimationProfile animationProfile){
        AlphaFadeUIAnimationProfile fadeUIAnimationProfile = (AlphaFadeUIAnimationProfile)animationProfile;
        CanvasGroup canvasGroup = fadeUIAnimationProfile.CanvasGroup;

        float startingAlpha = fadeUIAnimationProfile.AlphaStart == -1 ? canvasGroup.alpha : Mathf.Clamp01(fadeUIAnimationProfile.AlphaStart);

        canvasGroup.alpha = startingAlpha;

        float current = 0;
        while(Mathf.Abs(canvasGroup.alpha - fadeUIAnimationProfile.AlphaEnd) > LERP_SNAP_DISTANCE){
            if(fadeUIAnimationProfile.ScaledDeltaTime){
                current += Time.deltaTime;
            }
            else{
                current += Time.unscaledDeltaTime;
            }

            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, fadeUIAnimationProfile.AlphaEnd, current / fadeUIAnimationProfile.AnimationDuration);

            yield return null;
        }

        canvasGroup.alpha = fadeUIAnimationProfile.AlphaEnd;

        UIAnimationFinished(fadeUIAnimationProfile, canvasGroup.gameObject);
    }

    /// A animation that lerps a canvas group's alpha value between to values, giving a pulsing effect.
    public static IEnumerator CanvasGroupAlphaPulseCoroutine(UIAnimationProfile animationProfile){
        AlphaPulseUIAnimationProfile pulseUIAnimationProfile = (AlphaPulseUIAnimationProfile)animationProfile;
        CanvasGroup canvasGroup = pulseUIAnimationProfile.CanvasGroup;

        float elapsedTime = 0f;
        float time = 0f; 

        // Use scaled or unscaled delta time
        float deltaTime = pulseUIAnimationProfile.ScaledDeltaTime ? Time.deltaTime : Time.unscaledDeltaTime;

        while (pulseUIAnimationProfile.AnimationDuration < 0 || elapsedTime < pulseUIAnimationProfile.AnimationDuration){
            // Create pulsing effect using PingPong and SmoothStep
            float t = Mathf.PingPong(time * pulseUIAnimationProfile.AnimationSpeed, 1f);
            t = Mathf.SmoothStep(0f, 1f, t);

            // Interpolate alpha based on the pulsing value (t)
            canvasGroup.alpha = Mathf.Lerp(pulseUIAnimationProfile.AlphaStart, pulseUIAnimationProfile.AlphaEnd, t);

            // Update time for pulsing and elapsed time for duration control
            time += deltaTime;
            elapsedTime += deltaTime;
            yield return null;
        }

        UIAnimationFinished(pulseUIAnimationProfile, canvasGroup.gameObject);
    }
}