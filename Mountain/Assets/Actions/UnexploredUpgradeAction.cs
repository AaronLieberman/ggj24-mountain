using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnexploredUpgradeAction : PlacementAction
{
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        var placementToSpawn = card.PlacementToSpawn != null ? card.PlacementToSpawn : card.UnrevealedPlacement;
        if (placementToSpawn == null)
            return;

        var instance = placement.GetComponentInParent<Tile>().SpawnPlacement(placementToSpawn);
        instance.RevealAction?.DoWork(worker, instance, card);
    }
}
