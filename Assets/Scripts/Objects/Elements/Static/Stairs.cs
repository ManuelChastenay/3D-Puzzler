using System.Collections;
using UnityEngine;

public class Stairs : Element
{
	[SerializeField] private float STAIRS_CLIMB_DURATION;

	public override void Initialize(Cube cube)
	{
		throw new System.NotImplementedException();
	}

	public override void OverrideMovementCommand(Player player, Vector3Int desiredMoveCube)
	{
		StartCoroutine(StairsCoroutine(player, desiredMoveCube));
	}

	public IEnumerator StairsCoroutine(Player player, Vector3Int desiredMoveCube)
	{
		throw new System.NotImplementedException();
		/*player.actionOngoing = true;
		player.timer = 0.0f;

		Vector3 baseWorldPos = player.transform.position;
		desiredMoveCube += ComputeTrueMoveCube(player);
		Vector3 moveToWorldPos = currentCube.GetWorldPosFromCubePos(desiredMoveCube);

		while (player.timer < STAIRS_CLIMB_DURATION) {
			float lerpT = Maths.PlayerEaseMovement(player.timer / STAIRS_CLIMB_DURATION);

			player.transform.position = baseWorldPos + ((moveToWorldPos - baseWorldPos) * lerpT);
			yield return new WaitForEndOfFrame();
		}

		player.transform.position = moveToWorldPos;
		player.CubePos = desiredMoveCube;
		player.actionOngoing = false;*/
	}

	public Vector3Int ComputeTrueMoveCube(Player player)
	{
		return Vector3Int.RoundToInt(-player.rotationPivot.transform.forward);
	}
}
