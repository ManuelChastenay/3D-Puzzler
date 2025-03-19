using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementCube : Element
{
	[SerializeField] private Material facesMat;
	public MeshRenderer mr;

	private Color color;

	public List<Face> faces = new List<Face>();
	public Cube linkedCube;

	public override void Initialize(Cube linkedCube)
	{
		this.linkedCube = linkedCube;

		Material mat = mr.sharedMaterial;
		mat = new Material(mat);
		color = linkedCube.color;
		color.a = .5f;
		mat.color = color;
		mr.sharedMaterial = mat;

		Material newMaterial = new Material(facesMat);
		newMaterial.SetColor("Color_BE5A53D8", color);
		newMaterial.SetVector("Vector2_EF5B598A", Vector3.one / 2 * linkedCube.size); //Tiling

		foreach (Face face in faces) {
			face.InitializeCameraTransforms();
			face.quad.sharedMaterial = newMaterial;
		}
	}

	public Dictionary<Faces, List<Transform>> GetFaceCamerasMap()
	{
		Dictionary<Faces, List<Transform>> FCMap = new();
		foreach (Face face in faces) {
			FCMap.Add(face.face, face.GetCamHolders());
		}
		return FCMap;
	}

	public override void OverrideMovementCommand(Player player, Vector3Int desiredMoveCube)
	{
		//TODO Differ between push elementCube or enter it
		ComputeNewCubePosition(player, out Face toFloorCubeFace, out Face toEnteredCubeFace);

		LifeCycleManager lcm = LifeCycleManager.Instance;
		lcm.AddCoroutineToCurrentStage(EnterElementCubeCoroutine(player, desiredMoveCube, toFloorCubeFace, toEnteredCubeFace));
		lcm.StartNewLifeCycle(player);
	}

	private void ComputeNewCubePosition(Player player, out Face toFloorCubeFace, out Face toEnteredCubeFace)
	{
		Face fromCubeFace = player.currentFace;
		Face toFloorElementCubeFace = faces.First(f => Vector3.Angle(f.transform.forward, fromCubeFace.transform.forward) == 0.0f);

		//TODO Pas très robuste
		Face toEnteredElementCubeFace = faces[0];
		float minDist = float.PositiveInfinity;
		foreach (Face face in faces) {
			float faceDist = Vector3.Distance(face.quad.transform.position, player.rotationPivot.transform.position);
			if (faceDist < minDist) {
				minDist = faceDist;
				toEnteredElementCubeFace = face;
			}
		}

		toFloorCubeFace = linkedCube.faces.First(f => f.face == toFloorElementCubeFace.face);
		toEnteredCubeFace = linkedCube.faces.First(f => f.face == toEnteredElementCubeFace.face);
	}

	public IEnumerator EnterElementCubeCoroutine(Player player, Vector3Int desiredMoveCube, Face toFloorCubeFace, Face toEnteredCubeFace, float TTC = 0.0f)
	{
		CameraController.Instance.OnCubeSwitched(this, toFloorCubeFace.face);

		if (TTC == 0.0f) TTC = 1.00f;
		float timer = 0.0f;

		Vector3Int toCubePosInt = GetCubeArrivalPosition(toFloorCubeFace, toEnteredCubeFace);
		Vector3 fromPos = player.transform.position;

		Face faceFacingEntry = player.currentCube.faces.First(f => f.transform.forward == player.CubePos - CubePos);
		Vector3Int ecOffsetInt = GetCubeArrivalPosition(player.currentFace, faceFacingEntry);
		Vector3 ecOffset = new Vector3(ecOffsetInt.x, ecOffsetInt.y, ecOffsetInt.z) / linkedCube.size;
		Vector3 ecPos = currentCube.GetWorldPosFromCubePos(desiredMoveCube) + ecOffset;

		Quaternion toRot = toFloorCubeFace.transform.localRotation;

		Vector3 baseScale = player.transform.localScale;
		Vector3 ecScale = baseScale / linkedCube.size;


		while (timer < TTC) {
			timer += Time.deltaTime;
			float lerpT = Maths.PlayerEaseMovement(timer / TTC);

			player.transform.position = fromPos + ((ecPos - fromPos) * lerpT);
			player.transform.localScale = baseScale + ((ecScale - baseScale) * lerpT);
			yield return new WaitForEndOfFrame();
		}

		player.currentCube.DelinkElement(player);
		linkedCube.LinkElement(player, toFloorCubeFace.face);

		player.CubePos = toCubePosInt;
		player.transform.position = player.currentCube.GetWorldPosFromCubePos(player.CubePos);
		player.rotationPivot.transform.rotation = toRot;
		player.transform.localScale = baseScale;
	}


	//Manu: Scusez j'ai voulu rendre ça générique mais jpense jsuis trop cave pour ça so vla 36 switch case xd.
	//Also si ça bug sorry j'ai pt pas tout testé les cas.
	public Vector3Int GetCubeArrivalPosition(Face floor, Face enteredFrom)
	{
		int s = linkedCube.size - 1;
		int hs = s / 2;

		switch (floor.face) {
			case Faces.Top: {
				switch (enteredFrom.face) {
					case Faces.Top:		return new Vector3Int(hs, s, hs);
					case Faces.Bottom:	return new Vector3Int(hs, 0, hs);
					case Faces.Front:	return new Vector3Int(s, s, hs);
					case Faces.Back:	return new Vector3Int(0, s, hs);
					case Faces.Right:	return new Vector3Int(hs, s, 0);
					case Faces.Left:	return new Vector3Int(hs, s, s);
				}
				break;
			};
			case Faces.Bottom: {
				switch (enteredFrom.face) {
					case Faces.Top:		return new Vector3Int(hs, s, hs);
					case Faces.Bottom:	return new Vector3Int(hs, 0, hs);
					case Faces.Front:	return new Vector3Int(s, 0, hs);
					case Faces.Back:	return new Vector3Int(0, 0, hs);
					case Faces.Right:	return new Vector3Int(hs, 0, 0);
					case Faces.Left:	return new Vector3Int(hs, 0, s);
				}
				break;
			};
			case Faces.Front: {
				switch (enteredFrom.face) {
					case Faces.Top:		return new Vector3Int(s, s, hs);
					case Faces.Bottom:	return new Vector3Int(s, 0, hs);
					case Faces.Front:	return new Vector3Int(s, hs, hs);
					case Faces.Back:	return new Vector3Int(0, hs, hs);
					case Faces.Right:	return new Vector3Int(s, hs, 0);
					case Faces.Left:	return new Vector3Int(s, hs, s);
				}
				break;
			};
			case Faces.Back: {
				switch (enteredFrom.face) {
					case Faces.Top:		return new Vector3Int(0, s, hs);
					case Faces.Bottom:	return new Vector3Int(0, 0, hs);
					case Faces.Front:	return new Vector3Int(s, hs, hs);
					case Faces.Back:	return new Vector3Int(0, hs, hs);
					case Faces.Right:	return new Vector3Int(0, hs, 0);
					case Faces.Left:	return new Vector3Int(0, hs, s);
				}
				break;
			};
			case Faces.Right: {
				switch (enteredFrom.face) {
					case Faces.Top:		return new Vector3Int(hs, s, 0);
					case Faces.Bottom:	return new Vector3Int(hs, 0, 0);
					case Faces.Front:	return new Vector3Int(s, hs, 0);
					case Faces.Back:	return new Vector3Int(0, hs, 0);
					case Faces.Right:	return new Vector3Int(hs, hs, 0);
					case Faces.Left:	return new Vector3Int(hs, hs, s);
				}
				break;
			};
			case Faces.Left: {
				switch (enteredFrom.face) {
					case Faces.Top:		return new Vector3Int(hs, s, s);
					case Faces.Bottom:	return new Vector3Int(hs, 0, s);
					case Faces.Front:	return new Vector3Int(s, hs, s);
					case Faces.Back:	return new Vector3Int(0, hs, s);
					case Faces.Right:	return new Vector3Int(hs, hs, 0);
					case Faces.Left:	return new Vector3Int(hs, hs, s);
				}
				break;
			};
		}

		throw new Exception("Face doesn't exist.");



		//Tried out code.
		/*
		Quaternion floorRot = floor.transform.rotation * Maths.baseRotInv;
		Quaternion enteredFromRot = enteredFrom.transform.rotation * Maths.baseRotInv;

		Vector3Int move = Vector3Int.RoundToInt(enteredFromRot * new Vector3Int(halfSize, 0, 0));
		Vector3Int enteredFaceAddedOffset = Vector3Int.RoundToInt(enteredFromRot * new Vector3Int(0, size, 0));

		Debug.Log(move);

		if(!enteredFrom.IsStartFace()) move += enteredFaceAddedOffset;
		Debug.Log(move);

		move = Vector3Int.RoundToInt(floorRot * move);
		Debug.Log(move);

		move = new Vector3Int(Mathf.Abs(move.x), Mathf.Abs(move.y), Mathf.Abs(move.z));
		Debug.Log(move);

		return move;
		*/
	}
}
