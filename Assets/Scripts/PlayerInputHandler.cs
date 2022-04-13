using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

public class PlayerInputHandler : StarterAssets.StarterAssetsInputs
{

	public bool crouch;
	
	public bool primary;

	//public bool swapWeapon;
	//public int weaponSwap;

	//protected InputAction SwitchWeapon;
	public float scroll;

	public bool secondary;

	public void OnCrouch(InputValue value) //From Input Manager?
	{
		CrouchInput(value.isPressed);
	}

	public void CrouchInput(bool newCrouchState)
	{
		crouch = newCrouchState;
	}

	public void OnPrimaryAction(InputValue value)
	{
		GameManager.PlayerFPAnimHandler.OnPrimaryAction();
		//primary = value.isPressed;
	}

	//Interact
	public void OnSecondaryAction(InputValue value)
	{
		SecondaryInput(value.isPressed);
	}

	public void OnSwitchWeapon(InputValue value)
	{
		if (cursorInputForLook) {
			scroll = value.Get<float>();
		}

	}

	public void OnNextWeapon(InputValue value)
	{
		
	}

	public void OnPreviousWeapon(InputValue value)
	{

	}

	public void SecondaryInput(bool newSecondaryState)
	{
		secondary = newSecondaryState;
	}

	//public void OnSprint(InputValue value)
	//{
	//	SprintInput(value.isPressed);
	//}


}