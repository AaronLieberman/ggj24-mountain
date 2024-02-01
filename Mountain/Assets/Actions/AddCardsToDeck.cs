using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardsToDeck : PlacementAction
{

    public int NumberOfFieldsToAdd;
    public int NumberOfForestsToAdd;
    public int NumberOfSettlementsToAdd;
    public int NumberOfSwampsToAdd;
    public int NumberOfWastelandsToAdd;

    public int NumberOfRandomTypeToAdd;

    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        PlacementPoolManager.Instance.AddToDeckFromCurrentPool(NumberOfRandomTypeToAdd);
	}
}
