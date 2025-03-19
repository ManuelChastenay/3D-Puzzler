using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public bool actionOngoing = false;

	[SerializeField] private float MOVE_DURATION;
	[SerializeField] private Camera cam;
	[SerializeField] private Player player;
	private int camIndex = 0;

	private float timer;
	private Dictionary<Faces, List<Transform>> currentCubeFCMap;

	public static CameraController Instance;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		camIndex = 0;

		currentCubeFCMap = player.currentCube.GetFaceCamerasMap();
		Transform ct = currentCubeFCMap[player.currentFace.face][camIndex];

		transform.parent = ct;
		transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		cam.orthographicSize = player.currentCube.size;
	}

	private void Update()
	{
		timer += Time.deltaTime;
	}

	public void ManageRotation(int moveCameraInput)
	{
		List<Transform> camTransforms = currentCubeFCMap[player.currentFace.face];
		camIndex = (camTransforms.Count + camIndex + moveCameraInput) % camTransforms.Count;

		StartCoroutine(MoveCameraCircleCoroutine(camTransforms, camTransforms[camIndex]));
	}

	public void OnFaceSwitched(Face fromFace, Face toFace)
	{
		if (actionOngoing) return;

		//TODO Wont work if not facing desired side. Implement graph lookup table for closest cam facing face?
		List<Transform> FFCT = currentCubeFCMap[fromFace.face];
		List<Transform> TFCT = currentCubeFCMap[toFace.face];
		Transform ct = TFCT.Find(ct => ct.position == transform.position);
		camIndex = TFCT.IndexOf(ct);

		StartCoroutine(RotateCameraOnAxisCoroutine(ct));
	}

	public void OnCubeSwitched(ElementCube toCube, Faces toCubeFloor)
	{
		if (actionOngoing) return;

		currentCubeFCMap = player.currentCube.GetFaceCamerasMap();
		Dictionary<Faces, List<Transform>> toElementCubeFaceCTMap = toCube.GetFaceCamerasMap(); //Pas nécessairement dans même rotation que les Cubes.
		Dictionary<Faces, List<Transform>> toCubeFaceCTMap = toCube.linkedCube.GetFaceCamerasMap();

		Transform currentCT = currentCubeFCMap[player.currentFace.face][camIndex];
		Transform elementCubeCT = toElementCubeFaceCTMap[toCubeFloor].First(ct => ct.rotation == currentCT.rotation);
		Transform toCT = toCubeFaceCTMap[toCubeFloor][toElementCubeFaceCTMap[toCubeFloor].IndexOf(elementCubeCT)];

		StartCoroutine(MoveCameraChangeCubeCoroutine(toCube, currentCT, elementCubeCT, toCT));
	}

	public IEnumerator MoveCameraCircleCoroutine(List<Transform> camTransforms, Transform toTransform)
	{
		actionOngoing = true;
		timer = 0.0f;

		Tuple<Vector3, Vector3, float> tuple = Maths.ComputeCircle(camTransforms.Select(ct => ct.position).ToList());
		Vector3 center = tuple.Item1;
		Vector3 fixedAxis = tuple.Item2.normalized;

		transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
		toTransform.GetPositionAndRotation(out Vector3 endPos, out Quaternion endRot);

		Vector3 startVec = startPos - center;
		Vector3 endVec = endPos - center;
		float startAngle = Vector3.SignedAngle(startVec, endVec, fixedAxis);

		while (timer < MOVE_DURATION) {
			float lerpT = Maths.PlayerEaseMovement(timer / MOVE_DURATION);
			float currentAngle = Mathf.Lerp(0, startAngle, lerpT);

			Vector3 rotatedPos = Quaternion.AngleAxis(currentAngle, fixedAxis) * startVec;
			transform.SetPositionAndRotation(center + rotatedPos, Quaternion.Lerp(startRot, endRot, lerpT));

			yield return new WaitForEndOfFrame();
		}

		transform.parent = toTransform;
		transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		actionOngoing = false;
	}

	public IEnumerator RotateCameraOnAxisCoroutine(Transform toTransform)
	{
		actionOngoing = true;
		timer = 0.0f;

		Quaternion startRot = transform.rotation;
		Quaternion endRot = toTransform.rotation;

		while (timer < MOVE_DURATION) {
			float lerpT = Maths.PlayerEaseMovement(timer / MOVE_DURATION);
			transform.rotation = Quaternion.Lerp(startRot, endRot, lerpT);

			yield return new WaitForEndOfFrame();
		}

		transform.parent = toTransform;
		transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		actionOngoing = false;
	}

	public IEnumerator MoveCameraChangeCubeCoroutine(ElementCube ec, Transform curr, Transform elementCube, Transform to)
	{
		actionOngoing = true;
		timer = 0.0f;

		Color baseColor = ec.mr.sharedMaterial.color;
		Color endColor = ec.mr.sharedMaterial.color;
		Color color = ec.mr.sharedMaterial.color;
		endColor.a = 0;

		while (timer < 1.00f) {
			float lerpT = Maths.PlayerEaseMovement(timer / 1.00f);

			color.a = baseColor.a + (endColor.a - baseColor.a) * lerpT;
			ec.mr.sharedMaterial.color = color;

			transform.position = curr.position + (elementCube.position - curr.position) * lerpT;
			cam.orthographicSize = player.currentCube.size + (1 - player.currentCube.size) * lerpT;
			yield return new WaitForEndOfFrame();
		}

		transform.parent = to;
		transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		cam.orthographicSize = ec.linkedCube.size;
		actionOngoing = false;

		currentCubeFCMap = ec.linkedCube.GetFaceCamerasMap();
		ec.mr.sharedMaterial.color = baseColor;
	}
}
