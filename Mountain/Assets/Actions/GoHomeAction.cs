using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoHomeAction : PlacementAction
{
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        worker.ClearWorkerPlans();
    }
}
