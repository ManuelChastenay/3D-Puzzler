using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(CubeCreator))]
public class CubeCreatorEditor : Editor
{
	private readonly int[] selectableCubeSizes = { 3, 5, 7, 9, 11 };

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		CubeCreator cubeCreator = (CubeCreator)target;

		int currentIndex = Mathf.Max(0, System.Array.IndexOf(selectableCubeSizes, cubeCreator.cubeSize));
		int newIndex = EditorGUILayout.Popup("Cube size", currentIndex, selectableCubeSizes.Select(v => v.ToString()).ToArray());

		if (newIndex != currentIndex) {
			cubeCreator.cubeSize = selectableCubeSizes[newIndex];
			EditorUtility.SetDirty(cubeCreator);
		}

		if(GUILayout.Button("Create cube with specified parameters")) {
			cubeCreator.CreateNewCube();
		}
	}
}
