using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum Faces { Top, Bottom, Front, Back, Right, Left }

public class Face : MonoBehaviour
{
	public List<Transform> camTransforms;
	[SerializeField] private Cube parentCube;
	[SerializeField] private ElementCube parentElementCube;
	public MeshRenderer quad;
	public Faces face;

	public bool initialized = false;

	public void InitializeCameraTransforms()
	{
		int size = (parentCube != null) ? parentCube.size : 1;
		GameObject center = (parentCube != null) ? parentCube.center : parentElementCube.rotationPivot;

		float offset = 10 + size / 2.0f;
		List<Vector3> positions = GetCamPositions(offset);

		for(int i = 0; i < camTransforms.Count; i++) {

			camTransforms[i].localPosition = positions[i];
			camTransforms[i].LookAt(center.transform, - transform.forward);
		}

		initialized = true;
	}

	public List<Transform> GetCamHolders()
	{
		return camTransforms;
	}

	public bool IsStartFace()
	{
		return face == Faces.Back || face == Faces.Right || face == Faces.Bottom;
	}

	private List<Vector3> GetCamPositions(float offset)
	{
		return new List<Vector3>() { 
			new(-offset, -offset, -offset), 
			new(offset, -offset, -offset), 
			new(offset, offset, -offset), 
			new(-offset, offset, -offset) 
		};
	}




#if UNITY_EDITOR
	public void DrawCamTrajectory()
	{
		if (!initialized) return;

		Handles.color = Color.red;
		foreach (Transform cam in camTransforms) {
			Handles.DrawSolidDisc(cam.transform.position, Vector3.forward, 0.2f);
			Handles.DrawSolidDisc(cam.transform.position, Vector3.right, 0.2f);
		}

		Handles.color = Color.yellow;
		Tuple<Vector3, Vector3, float> tuple = Maths.ComputeCircle(camTransforms.Select(ct => ct.transform.position).ToList());
		Handles.DrawWireDisc(tuple.Item1, tuple.Item2, tuple.Item3);
	}
#endif
}
