using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class IncreaseJourneyPlanSlots : PlacementAction
{
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        Utilities.GetRootComponent<GameManager>().IncreaseJourneyPlanSlots();
    }
}
