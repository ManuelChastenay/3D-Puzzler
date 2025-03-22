using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Player player;
	[SerializeField] private CameraController cam;

	private PlayerInput inputActions;


	private void Awake()
	{
		inputActions = new PlayerInput();
	}

	private void OnEnable()
	{
		inputActions.Player.Enable();
		inputActions.Player.Move.performed += OnMove;
		inputActions.Player.RotateCamera.performed += OnRotateCamera;
		inputActions.Player.Zoom.performed += OnZoom;
		//inputActions.Player.Restart.performed += OnRestart;
	}

	private void OnDisable()
	{
		inputActions.Player.Move.performed -= OnMove;
		inputActions.Player.RotateCamera.performed -= OnRotateCamera;
		inputActions.Player.Zoom.performed -= OnZoom;
		//inputActions.Player.Restart.performed -= OnRestart;
		inputActions.Player.Disable();
	}

	public Context GetContext()
	{
		return new Context(player.actionOngoing, cam.actionOngoing);
	}


	private void OnMove(InputAction.CallbackContext cbc)
	{
		Context context = GetContext();
		if (context.playerActionOngoing) return;

		Vector2 moveInput = cbc.ReadValue<Vector2>();
		Maths.Orthogonalize(ref moveInput);

		if (moveInput != Vector2.zero) {
			player.ManageMovement(context, moveInput);
		}
	}

	private void OnRotateCamera(InputAction.CallbackContext cbc)
	{
		Context context = GetContext();
		if (context.cameraActionOngoing) return;

		int moveCameraInput = (int)cbc.ReadValue<float>();

		cam.ManageRotation(moveCameraInput);
	}

	private void OnZoom(InputAction.CallbackContext cbc)
	{
		Vector2 scrollInput = cbc.ReadValue<Vector2>();
		float zoomInput = scrollInput.y;
		Debug.Log(zoomInput);
		cam.ManageZoom(zoomInput);
	}
}

public struct Context
{
	public bool playerActionOngoing;
	public bool cameraActionOngoing;

	public bool AnyActionOngoing { get => playerActionOngoing || cameraActionOngoing; }

	public Context(bool playerA, bool cameraA)
	{
		playerActionOngoing = playerA;
		cameraActionOngoing = cameraA;
	}
}