using System.Collections;
using UnityEngine;

public class Block : Element
{
	[SerializeField] private float KICK_BLOCK_DURATION;

	public override void Initialize(Cube cube)
	{
		throw new System.NotImplementedException();
	}

	public override void OverrideMovementCommand(Player player, Vector3Int desiredMoveCube)
	{
		LifeCycleManager lcm = LifeCycleManager.Instance;
		lcm.AddCoroutineToCurrentStage(Coroutines.BlockCoroutine(player, desiredMoveCube));
		lcm.StartNewLifeCycle(player);
	}
}

