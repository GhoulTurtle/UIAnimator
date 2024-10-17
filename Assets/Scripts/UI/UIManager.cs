using UnityEngine;

public class UIManager : MonoBehaviour{
	private void OnDestroy() {
		UIAnimator.ResetAnimationDictionary();
	}
}
