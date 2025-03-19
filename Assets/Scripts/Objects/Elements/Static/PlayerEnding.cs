using UnityEngine;

public class PlayerEnding : Element
{
	public override void Initialize(Cube cube)
	{
		throw new System.NotImplementedException();
	}

	public override void OverrideMovementCommand(Player player, Vector3Int desiredMoveCube)
	{
		Debug.LogError("T'as gagné mon maudit");
		throw new System.NotImplementedException();
	}
}
