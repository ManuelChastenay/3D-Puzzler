using System.Collections;
using System.Threading;
using UnityEngine;

public class Elevator : Element
{
	[SerializeField] private GameObject elevator;
	[SerializeField] private float ASCENSION_DURATION;
	[SerializeField] private int levelsToAscend; //TODO precompute possible values from position in editor script
	private float timer = 0.0f;

	public override void Initialize(Cube cube)
	{
		throw new System.NotImplementedException();
	}

	private void Update()
	{
		timer += Time.deltaTime;
	}

	public override void OverrideMovementCommand(Player player, Vector3Int desiredMoveCube)
	{
		LifeCycleManager lcm = LifeCycleManager.Instance;

		lcm.AddCoroutineToCurrentStage(Coroutines.SimpleMoveCoroutine(player, desiredMoveCube));
		
		lcm.IncrementCurrentStage();
		lcm.AddCoroutineToCurrentStage(AscensionCoroutine(player, desiredMoveCube));

		if (player.currentFace.face == currentFace.face) {
			lcm.AddCoroutineToFollowUpActions(DescentCoroutine(elevator.transform.position));
		} else {
			lcm.IncrementCurrentStage();
			lcm.AddCoroutineToCurrentStage(DescentCoroutine(elevator.transform.position));
		}

		lcm.StartNewLifeCycle(player);
	}

	public IEnumerator AscensionCoroutine(Player player, Vector3Int desiredMoveCube, float TTC = 0.0f)
	{
		if (TTC == 0.0f) TTC = ASCENSION_DURATION;
		float timer = 0.0f;

		Vector3 playerBaseWorldPos = player.transform.position;
		Vector3 elevatorBaseWorldPos = elevator.transform.position;

		desiredMoveCube += ComputeTrueMoveCube(player);
		Vector3 playerMoveToWorldPos = currentCube.GetWorldPosFromCubePos(desiredMoveCube);
		Vector3 elevatorMoveToWorldPos = elevatorBaseWorldPos + (playerMoveToWorldPos - playerBaseWorldPos);

		while (timer < ASCENSION_DURATION) {
			timer += Time.deltaTime;
			float lerpT = Maths.PlayerEaseMovement(timer / ASCENSION_DURATION);

			player.transform.position = playerBaseWorldPos + ((playerMoveToWorldPos - playerBaseWorldPos) * lerpT);
			elevator.transform.position = elevatorBaseWorldPos + ((elevatorMoveToWorldPos - elevatorBaseWorldPos) * lerpT);
			yield return new WaitForEndOfFrame();
		}

		player.transform.position = playerMoveToWorldPos;
		elevator.transform.position = elevatorMoveToWorldPos;

		player.CubePos = desiredMoveCube;
	}

	public IEnumerator DescentCoroutine(Vector3 elevatorBaseWorldPos)
	{
		timer = 0.0f;

		Vector3 elevatorCurrentWorldPos = elevator.transform.position;

		while (timer < ASCENSION_DURATION) {
			timer += Time.deltaTime;
			float lerpT = Maths.PlayerEaseMovement(timer / ASCENSION_DURATION);

			elevator.transform.position = elevatorCurrentWorldPos + ((elevatorBaseWorldPos - elevatorCurrentWorldPos) * lerpT);
			yield return new WaitForEndOfFrame();
		}
	}

	public Vector3Int ComputeTrueMoveCube(Player player)
	{
		return Vector3Int.RoundToInt(-rotationPivot.transform.forward * levelsToAscend);
	}
}