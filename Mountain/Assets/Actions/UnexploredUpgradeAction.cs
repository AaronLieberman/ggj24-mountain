using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnexploredUpgradeAction : PlacementAction
{
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        var placementToSpawn = card.IsRevealed
            ? (card.PlacementToSpawn != null ? card.PlacementToSpawn : card.UnrevealedPlacement)
            : (card.UnrevealedPlacement != null ? card.UnrevealedPlacement : card.PlacementToSpawn);
        if (placementToSpawn != null)
        {
            var instance = placement.GetComponentInParent<Tile>().SpawnPlacement(placementToSpawn);
            instance.RevealAction?.DoWork(worker, instance, card);
        }
    }
}
