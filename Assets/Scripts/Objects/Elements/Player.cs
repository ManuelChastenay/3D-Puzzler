using System;
using UnityEngine;

public class Player : Element
{
	public bool actionOngoing = false;

	public void ManageMovement(Context context, Vector2 moveInput)
	{
		Vector3Int desiredMoveCube = ComputeDesiredMove(moveInput);
		currentCube.DispatchMoveToElement(this, desiredMoveCube);
	}

	private Vector3Int ComputeDesiredMove(Vector2 moveInput)
	{
		Quaternion r = currentFace.transform.localRotation * Maths.baseRotInv;
		Vector3Int moveInput3D = Vector3Int.RoundToInt(r * new Vector3(moveInput.x, 0, moveInput.y));

		return CubePos + moveInput3D;
	}

	public override void OverrideMovementCommand(Player player, Vector3Int desiredMoveCube)
	{
		LifeCycleManager lcm = LifeCycleManager.Instance;
		lcm.AddCoroutineToCurrentStage(Coroutines.SimpleMoveCoroutine(this, desiredMoveCube));

		lcm.IncrementCurrentStage();
		lcm.AddCoroutineToCurrentStage(Coroutines.FallCoroutine(this, currentCube.GetFallPosition));

		lcm.StartNewLifeCycle(this);
	}


	//Unused
	public override void Initialize(Cube cube)
	{
		throw new NotImplementedException();
	}
}
