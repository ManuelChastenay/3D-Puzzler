using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Maths
{
	public static Quaternion baseRotInv = Quaternion.Inverse(Quaternion.Euler(90, 0, 0));

	public static float PlayerEaseMovement(float t)
    {
		if (t == 0 || t == 1) return t;
		return t < 0.5f ? Mathf.Pow(2, 20 * t - 10) / 2 : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
	}

	public static float CameraEaseMovement(float t)
	{
		Debug.Log("Add correct ease function");
		return PlayerEaseMovement(t);
	}

	public static Vector3 Lerp(Vector3 from, Vector3 to, float timer, float TTC, Func<float, float> easeFn)
	{
		return from + ((to - from) * easeFn(timer / TTC));
	}




	public static Vector3 Abs(Vector3 v)
	{
		return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
	}

	public static void Orthogonalize(ref Vector2 v)
	{
		if (v.x != 0.0f && v.y != 0.0f) v.x = 0.0f;
	}




	//Merci chatgpt
	public static Tuple<Vector3, Vector3, float> ComputeCircle(List<Vector3> points)
	{
		if (points.Count < 3) throw new Exception("Not enough points to compute circle.");

		Vector3 A = points[0];
		Vector3 B = points[1];
		Vector3 C = points[2];

		bool isXConstant = Mathf.Approximately(A.x, B.x) && Mathf.Approximately(B.x, C.x);
		bool isYConstant = Mathf.Approximately(A.y, B.y) && Mathf.Approximately(B.y, C.y);
		bool isZConstant = Mathf.Approximately(A.z, B.z) && Mathf.Approximately(B.z, C.z);

		if (!(isXConstant || isYConstant || isZConstant))
			throw new ArgumentException("One coordinate must be constant.");

		float x1 = A.x, y1 = A.y, z1 = A.z;
		float x2 = B.x, y2 = B.y, z2 = B.z;
		float x3 = C.x, y3 = C.y, z3 = C.z;

		float d, A1, A2, A3, ux, uy, uz;

		if (isXConstant) {
			d = 2 * (y1 * (z2 - z3) + y2 * (z3 - z1) + y3 * (z1 - z2));
			if (Mathf.Abs(d) < 1e-6f) throw new ArgumentException("Points are collinear, so a unique circle cannot be determined.");

			A1 = y1 * y1 + z1 * z1;
			A2 = y2 * y2 + z2 * z2;
			A3 = y3 * y3 + z3 * z3;
			ux = A.x;
			uy = (A1 * (z2 - z3) + A2 * (z3 - z1) + A3 * (z1 - z2)) / d;
			uz = (A1 * (y3 - y2) + A2 * (y1 - y3) + A3 * (y2 - y1)) / d;
		} else if (isYConstant) {
			d = 2 * (x1 * (z2 - z3) + x2 * (z3 - z1) + x3 * (z1 - z2));
			if (Mathf.Abs(d) < 1e-6f) throw new ArgumentException("Points are collinear, so a unique circle cannot be determined.");

			A1 = x1 * x1 + z1 * z1;
			A2 = x2 * x2 + z2 * z2;
			A3 = x3 * x3 + z3 * z3;
			ux = (A1 * (z2 - z3) + A2 * (z3 - z1) + A3 * (z1 - z2)) / d;
			uy = A.y;
			uz = (A1 * (x3 - x2) + A2 * (x1 - x3) + A3 * (x2 - x1)) / d;
		} else { // isZConstant
			d = 2 * (x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2));
			if (Mathf.Abs(d) < 1e-6f) throw new ArgumentException("Points are collinear, so a unique circle cannot be determined.");

			A1 = x1 * x1 + y1 * y1;
			A2 = x2 * x2 + y2 * y2;
			A3 = x3 * x3 + y3 * y3;
			ux = (A1 * (y2 - y3) + A2 * (y3 - y1) + A3 * (y1 - y2)) / d;
			uy = (A1 * (x3 - x2) + A2 * (x1 - x3) + A3 * (x2 - x1)) / d;
			uz = A.z;
		}

		Vector3 center = new Vector3(ux, uy, uz);
		Vector3 normal = Vector3.Cross(B - A, C - A).normalized;
		float radius = Vector3.Distance(center, A);

		return new Tuple<Vector3, Vector3, float>(center, normal, radius);
	}
}
