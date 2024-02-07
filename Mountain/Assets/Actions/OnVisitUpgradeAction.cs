using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnVisitUpgradeAction : PlacementAction
{
    public Placement UpgradeTo;
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if (UpgradeTo == null)
        {
            Debug.LogError("OnVisitUpgradeAction needs an UpgradeTo tile set, but doesn't have one");
            return;
        }

        placement.GetComponentInParent<Tile>().SpawnPlacement(UpgradeTo);
    }
}
