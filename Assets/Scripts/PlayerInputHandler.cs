using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

public class PlayerInputHandler : StarterAssets.StarterAssetsInputs
{

	public bool crouch;
	public bool primary;
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

	}

	public void OnSecondaryAction(InputValue value)
	{
		SecondaryInput(value.isPressed);
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