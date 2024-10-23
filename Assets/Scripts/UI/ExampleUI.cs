using UnityEngine;

public class ExampleUI : MonoBehaviour{
	[Header("UI References")]
	[SerializeField] private Transform uITransform;

	[Header("Required References")]
	[SerializeField] private UIAnimationSO bigStretchAnimationSO;
	[SerializeField] private UIAnimationSO defaultSizeStretchAnimationSO;

	private void Update() {
		if(Input.GetKeyDown(KeyCode.G)){
			UIAnimator.StartUIAnimation(this, bigStretchAnimationSO, uITransform, OnStretchAnimationFinished);
		}
		else if(Input.GetKeyDown(KeyCode.H)){
			UIAnimator.StartUIAnimation(this, defaultSizeStretchAnimationSO, uITransform, OnStretchAnimationFinished);
		}
		else if(Input.GetKeyDown(KeyCode.Escape)){
			if(UIAnimator.IsAnimationOfTypePlaying(this, UIAnimationType.Stretch)){
				UIAnimator.StopUIAnimationOnRunner(this, UIAnimationType.Stretch);
			}
		}
	}

	private void OnDestroy() {
		UIAnimator.StopUIAnimationOnRunner(this);
	}

	private void OnStretchAnimationFinished(){
		Debug.Log("Stretch has finished!");
	}
}


