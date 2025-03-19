using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Face), true), CanEditMultipleObjects]
public class FaceEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		List<Face> faces = targets.Select(t => (Face)t).ToList();

		if (GUILayout.Button($"Recompute cams positions")) {
			faces.ForEach(f => f.InitializeCameraTransforms());
		}
	}
}
