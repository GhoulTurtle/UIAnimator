using UnityEngine;

[CreateAssetMenu(menuName = "UI Animation/Alpha/Fade Animation", fileName = "NewFadeUIAnimationSO")]
public class AlphaFadeUIAnimationSO : UIAnimationSO{
	public AlphaFadeUIAnimationProfile animationProfile;

    public override UIAnimationProfile GetTemplateAnimationProfile() => animationProfile;
}
