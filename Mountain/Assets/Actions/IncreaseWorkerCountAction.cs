using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class IncreaseWorkerCountAction : PlacementAction
{
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        Utilities.GetRootComponent<Board>().AddWorkerAtHome();
    }
}
