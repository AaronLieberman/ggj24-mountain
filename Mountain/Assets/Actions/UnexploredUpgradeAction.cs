using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnexploredUpgradeAction : PlacementAction
{
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if (card.PlacementToSpawn == null)
            return;

        var instance = placement.GetComponentInParent<Tile>().SpawnPlacement(card.PlacementToSpawn);
        instance.RevealAction?.DoWork(worker, instance, card);
    }
}
