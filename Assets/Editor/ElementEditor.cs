using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Element), true)]
public class ElementEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		Element element = (Element)target;
		CustomElementInspector(element);

		if (element.currentCube == null) return;

		List<int> positions = element.currentCube.GetPossiblePositions();
		Vector3 newPosFloat = EditorGUILayout.Vector3Field("Position in cube", (Vector3)element.CubePos);

		int SnapToClosestInt(float value) => Mathf.RoundToInt(positions.OrderBy(pos => Mathf.Abs(pos - value)).FirstOrDefault());
		Vector3Int newPos = new Vector3Int(
			SnapToClosestInt(newPosFloat.x),
			SnapToClosestInt(newPosFloat.y),
			SnapToClosestInt(newPosFloat.z)
		);

		if (newPos != element.CubePos) {
			MoveBasePositionInCube(element, ref element.CubePos, newPos);
		}
	}

	private void MoveBasePositionInCube(Element element, ref Vector3Int var, Vector3Int val) {
		var = val;
		element.MoveBasePositionInCube();
		EditorUtility.SetDirty(element);
	}

	private void CustomElementInspector(Element element)
	{
		CurrentCubeInspector(element);
		CurrentFaceInspector(element);
		if(element is FaceSwitcher faceSwitcher) ToFaceInspector(faceSwitcher);
	}

	private void CurrentCubeInspector(Element element)
	{
		Cube cube = element.currentCube;
		element.currentCube = (Cube)EditorGUILayout.ObjectField("Current cube", element.currentCube, typeof(Cube), true);

		if (cube != element.currentCube) {
			cube.DelinkElement(element);
			element.currentCube.LinkElement(element);

			element.CubePos = new Vector3Int(0, 0, 0);
			element.MoveBasePositionInCube();

			EditorUtility.SetDirty(element);
		}
	}

	private void CurrentFaceInspector(Element element)
	{
		Face face = element.currentFace;
		element.currentFace = (Face)EditorGUILayout.ObjectField("Current face", element.currentFace, typeof(Face), true);

		if (face != element.currentFace) {
			element.rotationPivot.transform.rotation = element.currentFace.transform.localRotation;
			if (element is FaceSwitcher faceSwitcher) faceSwitcher.toFace = null;

			EditorUtility.SetDirty(element);
		}
	}

	private void ToFaceInspector(FaceSwitcher faceSwitcher)
	{
		Face face = faceSwitcher.toFace;
		faceSwitcher.toFace = (Face)EditorGUILayout.ObjectField("To face", faceSwitcher.toFace, typeof(Face), true);

		if (face != faceSwitcher.toFace) {
			//TODO Simplify rotation
			/*if (Vector3.Cross(faceSwitcher.currentFace.transform.localRotation.eulerAngles, 
					faceSwitcher.toFace.transform.localRotation.eulerAngles) == Vector3.zero) {
				Debug.LogError("Cannot travel between two colinear faces");
				faceSwitcher.toFace = null;
				return;
			}

			Quaternion correction = Quaternion.identity;
			Faces curr = faceSwitcher.currentFace.face;
			if (curr == Faces.Bottom || curr == Faces.Top) correction = Quaternion.Euler(90, 0, 0);
			if (curr == Faces.Front || curr == Faces.Back) correction = Quaternion.Euler(90, 0, 0);
			if (curr == Faces.Right || curr == Faces.Left) correction = Quaternion.Euler(90, 0, 0);

			Quaternion relativeRotation = Quaternion.Inverse(faceSwitcher.currentFace.transform.rotation) * faceSwitcher.toFace.transform.rotation;
			faceSwitcher.rotationPivot.transform.rotation = correction * relativeRotation;*/

			EditorUtility.SetDirty(faceSwitcher);
		}
	}



	void OnSceneGUI()
	{
		Element element = (Element)target;
		if(element is ElementCube ec) {
			List<Face> faces = ec.faces;

			foreach (Face face in faces) {
				face.DrawCamTrajectory();
			}
		}
	}
}
