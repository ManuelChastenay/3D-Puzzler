using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cube))]
public class CubeEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		Cube cube = (Cube)target;
		EditorGUILayout.Space(15);

		cube.create = GUILayout.Toggle(cube.create, cube.create ? "Create mode" : "Add mode");

		if (cube.create) {
			cube.elementToCreate = (ElementsToCreate)EditorGUILayout.EnumPopup("Element to create", cube.elementToCreate);
			if (GUILayout.Button($"Create new element of type: {cube.elementToCreate.ToString()}")) {
				cube.CreateNewElement(cube.elementToCreate);
			}
		} 
		
		
		else {
			cube.elementToAdd = (ElementsToAdd)EditorGUILayout.EnumPopup("Element to add", cube.elementToAdd);
			if (cube.elementToAdd == ElementsToAdd.Cube) {
				cube.cubeToAdd = (Cube)EditorGUILayout.ObjectField("Cube to add", cube.cubeToAdd, typeof(Cube), true);
				if (cube.cubeToAdd != null && GUILayout.Button($"Add new element of type: {cube.elementToAdd.ToString()}")) {
					cube.AddElement(cube.elementToAdd);
				}
			}
		}
	}

	void OnSceneGUI()
	{
		Cube cube = (Cube)target;
		List<Face> faces = cube.faces;

		foreach (Face face in faces) {
			face.DrawCamTrajectory();
		}
	}
}
