using UnityEngine;

[CreateAssetMenu(menuName = "UI Animation/Sin Animation", fileName = "NewSinUIAnimationSO")]
public class SinUIAnimationSO : UIAnimationSO{
	public SinUIAnimationProfile animationProfile;

    public override UIAnimationProfile GetTemplateAnimationProfile() => animationProfile;
}
