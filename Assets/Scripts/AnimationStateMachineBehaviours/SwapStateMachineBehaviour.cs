using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapStateMachineBehaviour : StateMachineBehaviour
{
	bool swapped = false;
	//public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	////public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//	//base.OnStateEnter(animator, stateInfo, layerIndex);
	//	GameManager.PlayerFPAnimHandler.OnWeaponSwap();
	//}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		swapped = false;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (swapped)
			return;

		if (stateInfo.normalizedTime > 0.99f) {
			GameManager.PlayerFPAnimHandler.OnWeaponSwap();
			swapped = true;
		}

	}
}
