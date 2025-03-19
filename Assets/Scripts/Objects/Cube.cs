using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public GameObject center;
    public GameObject elementsHolder;
	public Light cubeLight;
	public List<Face> faces = new List<Face>();
	public List<Element> elements;

	[HideInInspector] public int size;
	[HideInInspector] public Color color;

	[HideInInspector] public bool create;
	[HideInInspector] public ElementsToCreate elementToCreate;
	[HideInInspector] public ElementsToAdd elementToAdd;
	[HideInInspector] public Cube cubeToAdd;


	public void InitializeParams(int size, Color color)
    {
        this.size = size;
        this.color = color;

		cubeLight.intensity = size;
		cubeLight.range = size;

		foreach(Face face in faces) face.InitializeCameraTransforms();
	}

	public Dictionary<Faces, List<Transform>> GetFaceCamerasMap()
	{
		Dictionary<Faces, List<Transform>> FCMap = new();
		foreach (Face face in faces) FCMap.Add(face.face, face.GetCamHolders());
		
		return FCMap;
	}

	public Vector3 GetWorldPosFromCubePos(Vector3Int pos) => transform.position + pos;

	public Vector3Int GetFallPosition(Element el)
	{
		Vector3Int move = el.CubePos;
		Quaternion r = el.currentFace.transform.localRotation * Maths.baseRotInv;
		Vector3Int preMove = move;

		while (!IsOutOfBounds(move)) {
			Element match = elements.Find(e => e.CubePos == move);

			if (match != null && match != el) {
				if (match.GetType() == typeof(ElementCube)) throw new NotImplementedException("Falling in ElementCube");
				if (match.GetType() == typeof(FaceSwitcher)) throw new NotImplementedException("Falling in FaceSwitcher");
				if (match.GetType() == typeof(Stairs)) throw new NotImplementedException("Falling in Stairs");

				if (match.GetType() == typeof(Block) || match.GetType() == typeof(MovableBlock)) return preMove;
				if (match.GetType() == typeof(PlayerEnding)) return preMove;
				if (match.GetType() == typeof(Elevator)) throw new NotImplementedException("Falling in Elevator");
			}

			preMove = move;
			move += Vector3Int.RoundToInt(r * new Vector3Int(0, -1, 0));
		}

		return preMove;
	}

	public void DispatchMoveToElement(Player player, Vector3Int move)
	{
		//TODO devrait retourner Block pour avoir une rétroaction.
		if(IsOutOfBounds(move)) return;

        Element match = elements.Find(e => e.CubePos == move);
		if(match != null) match.OverrideMovementCommand(player, move);
		else player.OverrideMovementCommand(player, move);
	}

	public bool IsOutOfBounds(Vector3Int move)
	{
		return move.x < 0 || move.y < 0 || move.z < 0 || move.x > size - 1 || move.y > size - 1 || move.z > size - 1;
	}

	public Element GetElementAtPos(Vector3Int move)
	{
		return elements.Find(e => e.CubePos == move);
	}

	public void LinkElement(Element element, Faces face = Faces.Bottom)
	{
		elements.Add(element);
		element.currentCube = this;
		element.currentFace = faces.Find(f => f.face == face);
		element.transform.parent = elementsHolder.transform;
	}

	public void DelinkElement(Element element)
	{
		if (elements.Contains(element)) elements.Remove(element);
	}


	//Called from editor scripts.
	public List<int> GetPossiblePositions() => Enumerable.Range(0, size).ToList();

	public void CreateNewElement(ElementsToCreate newElement)
	{
		GameObject prefab = ElementsPrefabs.Instance.GetPrefab(newElement);

		GameObject elementGO = Instantiate(prefab, transform.position, Quaternion.identity);
		elementGO.transform.parent = elementsHolder.transform;
		elementGO.name = newElement.ToString();

		Element el = elementGO.GetComponent<Element>();
		if (el == null) throw new Exception("Create prefab has no associated Element script.");

		el.currentCube = this;
		el.currentFace = faces.Find(f => f.face == Faces.Bottom);
		elements.Add(el);
	}

	public void AddElement(ElementsToAdd addedElement)
	{
		if(addedElement == ElementsToAdd.Cube && cubeToAdd == this) {
			//If addedEl = cube, check between other or recursion.
			Debug.LogError("Recursion not implemented yet.");
			return;
		}

		GameObject prefab = ElementsPrefabs.Instance.GetPrefab(addedElement);

		GameObject elementGO = Instantiate(prefab, transform.position, Quaternion.identity);
		elementGO.transform.parent = elementsHolder.transform;
		elementGO.name = addedElement.ToString();

		ElementCube el = elementGO.GetComponent<ElementCube>();
		if (el == null) throw new Exception("Create prefab has no associated ElementCube script.");

		el.currentCube = this;
		el.currentFace = faces.Find(f => f.face == Faces.Bottom);
		el.Initialize(cubeToAdd);
		elements.Add(el);
	}
}
