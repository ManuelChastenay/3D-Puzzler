using System.Collections;
using UnityEngine;

public class FaceSwitcher : Element
{
	[SerializeField] private float FACE_SWITCH_DURATION;
	[HideInInspector] public Face toFace; //TODO Dynamic attribution

	public override void Initialize(Cube cube)
	{
		throw new System.NotImplementedException();
	}

	public override void OverrideMovementCommand(Player player, Vector3Int desiredMoveCube)
	{
		LifeCycleManager lcm = LifeCycleManager.Instance;

		if (player.currentFace == currentFace || player.currentFace == toFace)
			lcm.AddCoroutineToCurrentStage(FaceSwitchCoroutine(player, desiredMoveCube));
		else
			lcm.AddCoroutineToCurrentStage(Coroutines.BlockCoroutine(player, desiredMoveCube));

		lcm.StartNewLifeCycle(player);
	}

	public IEnumerator FaceSwitchCoroutine(Player player, Vector3Int desiredMoveCube, float TTC = 0.0f)
	{
		Face fromFace = player.currentFace;
		Face toFace = player.currentFace == currentFace ? this.toFace : this.currentFace;
		CameraController.Instance.OnFaceSwitched(fromFace, toFace);

		if (TTC == 0.0f) TTC = FACE_SWITCH_DURATION;
		float timer = 0.0f;

		Vector3 baseWorldPos = player.transform.position;
		desiredMoveCube += ComputeTrueMoveCube(player);
		Vector3 moveToWorldPos = currentCube.GetWorldPosFromCubePos(desiredMoveCube);

		Quaternion startRot = player.rotationPivot.transform.rotation;
		Quaternion endRot = toFace.transform.localRotation;

		while (timer < TTC) {
			timer += Time.deltaTime;
			float lerpT = Maths.PlayerEaseMovement(timer / TTC);

			player.transform.position = baseWorldPos + ((moveToWorldPos - baseWorldPos) * lerpT);
			player.rotationPivot.transform.rotation = Quaternion.Lerp(startRot, endRot, lerpT);
			yield return new WaitForEndOfFrame();
		}

		player.transform.position = moveToWorldPos;
		player.rotationPivot.transform.rotation = endRot;

		player.currentFace = toFace;
		player.CubePos = desiredMoveCube;
	}

	public Vector3Int ComputeTrueMoveCube(Player player)
	{
		return Vector3Int.RoundToInt(-player.rotationPivot.transform.forward);
	}
}
