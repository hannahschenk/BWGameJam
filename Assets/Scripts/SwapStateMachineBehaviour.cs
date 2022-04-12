using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapStateMachineBehaviour : StateMachineBehaviour
{

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		GameManager.PlayerFPAnim.OnWeaponSwap();
	}

}
