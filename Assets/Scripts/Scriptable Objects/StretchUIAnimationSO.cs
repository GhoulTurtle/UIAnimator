using UnityEngine;

[CreateAssetMenu(menuName = "UI Animation/Stretch Animation", fileName = "NewStretchUIAnimationSO")]
public class StretchUIAnimationSO : UIAnimationSO{
	public StretchUIAnimationProfile animationProfile;

    public override UIAnimationProfile GetTemplateAnimationProfile() => animationProfile;
}
