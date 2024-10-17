using UnityEngine;

[CreateAssetMenu(menuName = "UI Animation/Alpha/Pulse Animation", fileName = "NewPulseUIAnimationSO")]
public class AlphaPulseUIAnimationSO : UIAnimationSO{
	public AlphaPulseUIAnimationProfile animationProfile;

    public override UIAnimationProfile GetTemplateAnimationProfile() => animationProfile;
}
