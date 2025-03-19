using log4net.Util;
using System;
using System.Collections;
using UnityEngine;

public class Coroutines
{
	[SerializeField] private static float simpleMoveLength = 0.25f;
	[SerializeField] private static float blockLength = 0.25f;
	[SerializeField] private static float fallLength = 0.10f;

	//Stage 1
	public static IEnumerator SimpleMoveCoroutine(Element el, Vector3Int move, float TTC = 0.0f)
	{
		if (TTC == 0.0f) TTC = simpleMoveLength;
		float timer = 0.0f;

		Vector3 baseWorldPos = el.transform.position;
		Vector3 moveToWorldPos = el.currentCube.GetWorldPosFromCubePos(move);

		while (timer < TTC) {
			timer += Time.deltaTime;
			el.transform.position = Maths.Lerp(baseWorldPos, moveToWorldPos, timer, TTC, Maths.PlayerEaseMovement);
			yield return new WaitForEndOfFrame();
		}
		el.transform.position = moveToWorldPos;
		el.CubePos = move;
	}

	public static IEnumerator BlockCoroutine(Element el, Vector3Int move, float TTC = 0.0f)
	{
		if (TTC == 0.0f) TTC = blockLength;
		float timer = 0.0f;

		Vector3 baseWorldPos = el.transform.position;
		Vector3 moveToWorldPos = el.currentCube.GetWorldPosFromCubePos(move);

		while (timer < TTC) {
			timer += Time.deltaTime;
			float lerpT = Maths.PlayerEaseMovement(timer / TTC);

			if (timer <= TTC / 2) {
				el.transform.position = baseWorldPos + ((moveToWorldPos - baseWorldPos) * lerpT);
			} else {
				el.transform.position = baseWorldPos + ((moveToWorldPos - baseWorldPos) * (1 - lerpT));
			}
			yield return new WaitForEndOfFrame();
		}
		el.transform.position = baseWorldPos;
	}



	//Stage 2
	public static IEnumerator FallCoroutine(Element el, Func<Element, Vector3Int> getFallPosition, float TTC = 0.0f)
	{
		if (TTC == 0.0f) TTC = fallLength;
		float timer = 0.0f;

		Vector3Int fallPosition = getFallPosition(el);
		Vector3 baseWorldPos = el.transform.position;
		Vector3 moveToWorldPos = el.currentCube.GetWorldPosFromCubePos(fallPosition);
		float mag = (moveToWorldPos - baseWorldPos).magnitude;

		while (timer < TTC * mag) {
			timer += Time.deltaTime;
			el.transform.position = baseWorldPos + ((moveToWorldPos - baseWorldPos) * (timer / (TTC * mag)));
			yield return new WaitForEndOfFrame();
		}
		el.transform.position = moveToWorldPos;
		el.CubePos = fallPosition;
	}
}
