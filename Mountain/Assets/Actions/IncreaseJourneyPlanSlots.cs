using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class IncreaseJourneyPlanSlots : PlacementAction
{
    public int MaxLevelToIncreaseTo = 99;
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if (Utilities.GetRootComponent<GameManager>().MaxJourneySlots < MaxLevelToIncreaseTo)
        {
            Utilities.GetRootComponent<GameManager>().IncreaseJourneyPlanSlots();
        }
    }
}
