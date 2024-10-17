using UnityEngine;

[CreateAssetMenu(menuName = "UI Animation/Lerp Animation", fileName = "NewLerpUIAnimationSO")]
public class LerpUIAnimationSO : UIAnimationSO{
	public LerpUIAnimationProfile animationProfile;

    public override UIAnimationProfile GetTemplateAnimationProfile() => animationProfile;
}
