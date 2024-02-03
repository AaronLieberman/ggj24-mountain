using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DoMultipleActions : PlacementAction
{

    public List<PlacementAction> MultipleActions;
    public int NumberOfTimesToCallAllActions = 1;

    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        for (int i = 0; i < NumberOfTimesToCallAllActions; ++i)
        {
            foreach (var action in MultipleActions)
            {
                action.DoWork(worker, placement, card);
            }
        }
    }
}
