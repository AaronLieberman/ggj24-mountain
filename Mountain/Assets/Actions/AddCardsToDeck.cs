using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AddCardsToDeck : PlacementAction
{

    public int NumberOfFieldsToAdd;
    [FormerlySerializedAs("NumberOfForestsToAdd")]
    public int NumberOfWoodsToAdd;
    public int NumberOfSettlementsToAdd;
    public int NumberOfSwampsToAdd;
    public int NumberOfWastelandsToAdd;

    public int NumberOfRandomTypeToAdd;

    public bool ShouldBeRevealed = false;

    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        Utilities.GetRootComponent<PlacementPoolManager>().AddToDeckFromAllBiomesInPool(NumberOfFieldsToAdd, NumberOfWoodsToAdd, NumberOfSettlementsToAdd, NumberOfSwampsToAdd, NumberOfWastelandsToAdd, NumberOfRandomTypeToAdd, ShouldBeRevealed);

    }
}
