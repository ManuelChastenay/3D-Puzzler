using System;
using System.Collections;
using UnityEngine;

public class MovableBlock : Element
{
	[SerializeField] private float MOVE_DURATION;

	public override void Initialize(Cube cube)
	{
		throw new NotImplementedException();
	}

	public override void OverrideMovementCommand(Player player, Vector3Int desiredMoveCube)
	{
		Vector3Int pushDirection = desiredMoveCube - player.CubePos;
		Vector3Int cubeMove = pushDirection * 2 + player.CubePos;

		LifeCycleManager lcm = LifeCycleManager.Instance;

		Element elInCubePushSpot = currentCube.GetElementAtPos(cubeMove);
		if (!currentCube.IsOutOfBounds(cubeMove) && elInCubePushSpot == null /*TODO push dans ElementCube/Elevator*/) {
			lcm.AddCoroutineToCurrentStage(Coroutines.SimpleMoveCoroutine(player, desiredMoveCube));
			lcm.AddCoroutineToCurrentStage(PushCoroutine(cubeMove));

			lcm.IncrementCurrentStage();
			lcm.AddCoroutineToCurrentStage(Coroutines.FallCoroutine(this, currentCube.GetFallPosition));
		} else {
			lcm.AddCoroutineToCurrentStage(Coroutines.BlockCoroutine(player, desiredMoveCube));
		}

		lcm.StartNewLifeCycle(player);
	}

	public IEnumerator PushCoroutine(Vector3Int blockMove, float TTC = 0.0f)
	{
		if (TTC == 0.0f) TTC = MOVE_DURATION;
		float timer = 0.0f;

		Vector3 blockBaseWorldPos = transform.position;
		Vector3 blockMoveToWorldPos = currentCube.GetWorldPosFromCubePos(blockMove);

		while (timer < MOVE_DURATION) {
			timer += Time.deltaTime;
			transform.position = Maths.Lerp(blockBaseWorldPos, blockMoveToWorldPos, timer, TTC, Maths.PlayerEaseMovement);
			yield return new WaitForEndOfFrame();
		}
		transform.position = blockMoveToWorldPos;
		CubePos = blockMove;
	}
}
