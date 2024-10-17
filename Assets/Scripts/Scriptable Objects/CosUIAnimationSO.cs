using UnityEngine;

[CreateAssetMenu(menuName = "UI Animation/Cos Animation", fileName = "NewCosUIAnimationSO")]
public class CosUIAnimationSO : UIAnimationSO{
    public CosUIAnimationProfile animationProfile;

    public override UIAnimationProfile GetTemplateAnimationProfile() => animationProfile;
}