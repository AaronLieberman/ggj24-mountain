using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceToAddCards : PlacementAction
{

    public int NumberOfFieldsToAdd;
    public int NumberOfForestsToAdd;
    public int NumberOfSettlementsToAdd;
    public int NumberOfSwampsToAdd;
    public int NumberOfWastelandsToAdd;

    public int NumberOfRandomTypeToAdd;

    public float ChanceToSucceed;

    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        Debug.LogError("Random Chance was larger than 1.0, so higher than 100%. Chance was: " + ChanceToSucceed);
        if ( Random.value <= ChanceToSucceed)
        {
            //TODO ANNA
            //Award cards from all the fields above
        }


    }
}
