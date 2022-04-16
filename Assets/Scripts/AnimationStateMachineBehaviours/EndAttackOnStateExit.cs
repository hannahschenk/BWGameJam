using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAttackOnStateExit : StateMachineBehaviour
{

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//base.OnStateExit(animator, stateInfo, layerIndex);
		//GameManager.PlayerFPAnimHandler.AttackEnd();
		GameManager.PlayerFPAnimHandler.AttackAbilityFinish();
	}

}
