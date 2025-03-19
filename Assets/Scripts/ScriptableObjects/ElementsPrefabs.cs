using System;
using UnityEngine;

public class ElementsPrefabs : ScriptableObject
{
    [SerializeField] private GameObject FaceSwitcher;
    [SerializeField] private GameObject Block;
    [SerializeField] private GameObject MovableBlock;
	[SerializeField] private GameObject Stairs;
    [SerializeField] private GameObject Elevator;
    [SerializeField] private GameObject PlayerEnding;

	[SerializeField] private GameObject ElementCube;

	private static ElementsPrefabs _instance;
	public static ElementsPrefabs Instance {
		get { 
			if(_instance == null)
				_instance = CreateInstance<ElementsPrefabs>();
			return _instance; 
		}
		set { _instance = value; }
	}

	public GameObject GetPrefab(ElementsToCreate newElement)
	{
		switch (newElement) {
			case ElementsToCreate.FaceSwitcher: return FaceSwitcher;
			case ElementsToCreate.Block: return Block;
			case ElementsToCreate.MovableBlock: return MovableBlock;
			case ElementsToCreate.Stairs: return Stairs;
			case ElementsToCreate.Elevator: return Elevator;
			case ElementsToCreate.PlayerEnding: return PlayerEnding;
			default: throw new Exception("Create prefab does not exist.");
		}
	}

	public GameObject GetPrefab(ElementsToAdd addedElement)
	{
		switch (addedElement) {
			case ElementsToAdd.Cube: return ElementCube;
			default: throw new Exception("Create prefab does not exist.");
		}
	}
}
