using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input : MonoBehaviour
{
	public bool AnalogMovement => analogMovement;
	public bool CursorLocked => cursorLocked;
	public bool CursorInputForLook => cursorInputForLook;
	public Vector2 MoveDirection => moveDirection;
	public Vector2 AimDirection => aimDirection;
	public bool IsPressingSprint => isPressingSprint;
	public bool IsPressingThrow => isPressingThrow;

	// Input on press events
	public Action OnPressLamp;
	public Action OnPressKnife;
	public Action OnPressWeaponAbility;
	public Action OnPressChangeThrow;
	public Action OnPressChangeWeapon;
	public Action OnPressReload;
	public Action OnPressJump;
	public Action OnPressThrow;

	// Input on press longer events
	public Action OnPressLongChangeWeapon;
	public Action OnPressLongReload;

	public Action OnHoldFire;
	public Action OnReleaseFire;
	public Action OnReleaseThrow;

	[Header("Movement Settings")]
	[SerializeField] private float timeToHoldChangeWeapon = 0.5f;
	[SerializeField] private float timeToHoldReload = 0.5f;
	[SerializeField] private bool analogMovement;

	[Header("Mouse Cursor Settings")]
	[SerializeField] private bool cursorLocked = true;
	[SerializeField] private bool cursorInputForLook = true;

	private Vector2 moveDirection;
	private Vector2 aimDirection;
	private bool isPressingSprint = false;
	private bool isPressingFire = false;
	private bool isPressingThrow = false;
	private bool isPressingChangeWeapon = false;
	private bool isPressingReload = false;

	private bool isHoldingChangeWeapon = false;
	private bool isHoldingReload = false;

	private float pressChangeWeaponElapsedTime = 0;
	private float pressReloadElapsedTime = 0;

	private void Update()
	{
		if (isPressingChangeWeapon)
			OnHoldChangeWeapon();

		if (isPressingReload)
			OnHoldReload();

		if (isPressingFire)
			OnHoldFire.Invoke();
	}

	//Value functions (call every changement of value)
	private void OnMove(InputValue value)
	{
		moveDirection = value.Get<Vector2>();
	}

	private void OnAim(InputValue value)
	{
		if (cursorInputForLook)
			aimDirection = value.Get<Vector2>();
	}

	//Pass Through functions (call every changement of button state)
	private void OnReload(InputValue value)
	{
		isPressingReload = value.isPressed;

		if (value.isPressed) // started
		{
			isHoldingReload = false;
			pressReloadElapsedTime = 0;
		}
		else // canceled
		{
			if (!isHoldingReload)
				OnPressReload.Invoke();
		}
	}

	private void OnHoldReload()
	{
		pressReloadElapsedTime += Time.deltaTime;

		if (pressReloadElapsedTime >= timeToHoldReload && !isHoldingReload)
		{
			isHoldingReload = true;
			OnPressLongReload.Invoke();
		}
	}

	private void OnSprint(InputValue value)
	{
		isPressingSprint = value.isPressed;
	}

	private void OnFire(InputValue value)
    {
		isPressingFire = value.isPressed;

		if (!value.isPressed)
			OnReleaseFire.Invoke();
	}

	private void OnChangeWeapon(InputValue value)
	{
		isPressingChangeWeapon = value.isPressed;

		if (value.isPressed) // started
        {
			isHoldingChangeWeapon = false;
			pressChangeWeaponElapsedTime = 0;
		}
		else // canceled
        {
			if (!isHoldingChangeWeapon)
				OnPressChangeWeapon.Invoke();
		}
	}

	private void OnHoldChangeWeapon()
	{
		pressChangeWeaponElapsedTime += Time.deltaTime;

		if (pressChangeWeaponElapsedTime >= timeToHoldChangeWeapon && !isHoldingChangeWeapon)
		{
			isHoldingChangeWeapon = true;
			OnPressLongChangeWeapon.Invoke();
		}
	}

	private void OnThrow(InputValue value)
	{
		if (value.isPressed)
			OnPressThrow.Invoke();
		else
			OnReleaseThrow.Invoke();
	}

	//Button functions
	private void OnJump(InputValue value)
	{
		OnPressJump.Invoke();
	}

	private void OnWeaponAbility(InputValue value)
	{
		OnPressWeaponAbility.Invoke();
	}

	private void OnChangeThrow(InputValue value)
	{
		OnPressChangeThrow.Invoke();
	}

	private void OnLamp(InputValue value)
	{
		OnPressLamp.Invoke();
	}

	private void OnKnife(InputValue value)
	{
		OnPressKnife.Invoke();
	}

	// Others
	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}