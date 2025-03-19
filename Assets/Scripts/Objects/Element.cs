using UnityEngine;


public abstract class Element : MonoBehaviour
{
	public GameObject rotationPivot;

	[HideInInspector] public Cube currentCube;
	[HideInInspector] public Face currentFace;
	[HideInInspector] public Vector3Int CubePos;

	public void MoveBasePositionInCube()
	{
		transform.position = currentCube.GetWorldPosFromCubePos(CubePos);
	}

	public abstract void Initialize(Cube cube);
	public abstract void OverrideMovementCommand(Player player, Vector3Int desiredMoveCube);
	//public abstract void OverrideFall(Player player, Vector3Int desiredMoveCube);
}

public enum ElementsToCreate
{
	FaceSwitcher, Block, MovableBlock, Stairs, Elevator, PlayerEnding
}

public enum ElementsToAdd
{
	Cube
}