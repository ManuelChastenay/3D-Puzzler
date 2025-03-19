using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCycleManager : MonoBehaviour
{
	private List<List<IEnumerator>> stages = new();
	private List<IEnumerator> followUps = new();
	private List<IEnumerator> currentFollowUpsToAdd = new();
	private Player player;
	private int currentStage = 0;
	private int activeCoroutines;

	public static LifeCycleManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	/**
	 * Tous les stages après le stage 1 devraient pouvoir se calculer à la volée. On ne précalcule pas les action.
	*/
	public void StartNewLifeCycle(Player player)
	{
		this.player = player;
		this.player.actionOngoing = true;

		if(currentFollowUpsToAdd.Count > 0) stages.Add(currentFollowUpsToAdd);
		StartStage();
	}

	private void StartStage(int activeStage = 0)
	{
		if (stages.Count <= activeStage) {
			EndLifeCycle();
			return;
		};

		List<IEnumerator> stage = stages[activeStage];
		activeCoroutines = stage.Count;

		stage.ForEach(cr => StartCoroutine(TrackCoroutine(cr, activeStage)));
	}

	private IEnumerator TrackCoroutine(IEnumerator cr, int activeStage)
	{
		yield return StartCoroutine(cr);

		activeCoroutines--;
		if (activeCoroutines == 0) {
			StartStage(activeStage + 1);
		}
	}

	private void EndLifeCycle()
	{
		stages.Clear();
		currentStage = 0;
		player.actionOngoing = false;
		player = null;

		if (followUps.Count > 0) {
			currentFollowUpsToAdd.AddRange(followUps);
			followUps.Clear();
		}
		return;
	}

	private void PrintStages()
	{
		int index = 0;
		foreach(List<IEnumerator> i in stages) {
			print($"index: {index}");
			foreach (IEnumerator j in i) {
				print($"j: {j}");
			}
			index++;
		}
	}



	public void AddCoroutineToCurrentStage(IEnumerator cr)
	{
		while (stages.Count <= currentStage) stages.Add(new List<IEnumerator>());
		stages[currentStage].Add(cr);
	}

	public void IncrementCurrentStage() => currentStage++;

	public void AddCoroutineToFollowUpActions(IEnumerator cr)
	{
		followUps.Add(cr);
	}
}
