using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class IncreaseHandSize : PlacementAction
{
    public int MaxLevelToIncreaseTo = 99;
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if (Utilities.GetRootComponent<Hand>().MaxHandSize < MaxLevelToIncreaseTo)
        {
            Utilities.GetRootComponent<Hand>().IncreaseHandSize();
        }
    }
}
