using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationController : MonoBehaviour{
	[Header("UI References")]
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private RectTransform animatedGroupTransform;
	[SerializeField] private List<Image> buttonImageList;
	[SerializeField] private Color buttonActivatedColor;
	[SerializeField] private Color buttonDeactiavtedColor;

	[Header("UI Animation References")]
	[SerializeField] private UIAnimationSO sinAnimation;
	[SerializeField] private UIAnimationSO cosAnimation;
	[SerializeField] private UIAnimationSO alphaFadeInAnimation;
	[SerializeField] private UIAnimationSO alphaFadeOutAnimation;
	[SerializeField] private UIAnimationSO alphaPulseAnimation;
	[SerializeField] private UIAnimationSO defaultSizeStretchAnimation;
	[SerializeField] private UIAnimationSO bigSizeStretchAnimation;

	[Header("Controller Variables")]
	[SerializeField] private float colorFeedbackTimeInSeconds = 0.5f;

	private void OnDestroy() {
		UIAnimator.StopUIAnimationOnRunner(this);
		StopAllCoroutines();
	}

	public void StopUIAnimations(){
		UIAnimator.StopUIAnimationOnRunner(this);
		for (int i = 0; i < buttonImageList.Count; i++){
			ChangeButtonColor(i, false);
		}

		//Reset the Animated UI
		canvasGroup.alpha = 1;
		animatedGroupTransform.offsetMin = Vector2.zero;
		animatedGroupTransform.offsetMax = Vector2.zero;
		animatedGroupTransform.localScale = Vector2.one;
	}

	public void ChangeButtonColor(int buttonIndex, bool isActive){
		if(buttonIndex < 0 || buttonIndex > buttonImageList.Count) return;
		
		Image buttonImage = buttonImageList[buttonIndex];

		buttonImage.color = isActive ? buttonActivatedColor : buttonDeactiavtedColor;
	}

	public bool IsButtonActive(int buttonIndex){
		Image buttonImage = buttonImageList[buttonIndex];

		return buttonImage.color == buttonActivatedColor;
	}

	public void ToggleSinAnimation(int buttonIndex){
		if(UIAnimator.IsAnimationOfTypePlaying(this, UIAnimationType.Sin)){
			UIAnimator.StopUIAnimationOnRunner(this, UIAnimationType.Sin);
			ChangeButtonColor(buttonIndex, false);
		}
		else{
			UIAnimator.StartUIAnimation(this, sinAnimation, animatedGroupTransform.transform, null);
			ChangeButtonColor(buttonIndex, true);
		}
	}

	public void ToggleCosAnimation(int buttonIndex){
		if(UIAnimator.IsAnimationOfTypePlaying(this, UIAnimationType.Cos)){
			UIAnimator.StopUIAnimationOnRunner(this, UIAnimationType.Cos);
			ChangeButtonColor(buttonIndex, false);
		}
		else{
			UIAnimator.StartUIAnimation(this, cosAnimation, animatedGroupTransform.transform, null);
			ChangeButtonColor(buttonIndex, true);
		}
	}

	public void ToggleAlphaPulseAnimation(int buttonIndex){
		if(UIAnimator.IsAnimationOfTypePlaying(this, UIAnimationType.AlphaPulse)){
			UIAnimator.StopUIAnimationOnRunner(this, UIAnimationType.AlphaPulse);
			ChangeButtonColor(buttonIndex, false);
		}
		else{
			UIAnimator.StartUIAnimation(this, alphaPulseAnimation, canvasGroup, null);
			ChangeButtonColor(buttonIndex, true);
		}
	}

	public void TriggerStretchAnimation(int buttonIndex){
		if(IsButtonActive(buttonIndex)){
			UIAnimator.StartUIAnimation(this, defaultSizeStretchAnimation, animatedGroupTransform.transform, null);
			ChangeButtonColor(buttonIndex, false);
		}
		else{
			UIAnimator.StartUIAnimation(this, bigSizeStretchAnimation, animatedGroupTransform.transform, null);
			ChangeButtonColor(buttonIndex, true);
		}
	}

	public void TriggerFadeInAnimation(int buttonIndex){
		if(UIAnimator.IsAnimationOfTypePlaying(this, UIAnimationType.AlphaPulse)) return;

		UIAnimator.StartUIAnimation(this, alphaFadeInAnimation, canvasGroup, null);
		StartCoroutine(ButtonFeedbackCoroutine(buttonIndex));
	}

	public void TriggerFadeOutAnimation(int buttonIndex){
		if(UIAnimator.IsAnimationOfTypePlaying(this, UIAnimationType.AlphaPulse)) return;

		UIAnimator.StartUIAnimation(this, alphaFadeOutAnimation, canvasGroup, null);
		StartCoroutine(ButtonFeedbackCoroutine(buttonIndex));
	}

	private IEnumerator ButtonFeedbackCoroutine(int buttonIndex){
		ChangeButtonColor(buttonIndex, true);
		yield return new WaitForSeconds(colorFeedbackTimeInSeconds);
		ChangeButtonColor(buttonIndex, false);
	}
}
