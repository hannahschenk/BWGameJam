using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateItemsOnEnterState : StateMachineBehaviour
{

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		//GameManager.PlayerFPAnimHandler.UpdateItemsState();
		GameManager.PlayerFPAnimHandler.UpdateItemsStateAndWield();
	}

}
