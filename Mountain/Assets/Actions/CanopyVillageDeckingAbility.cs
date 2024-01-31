using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanopyVillageDeckingAbility : PlacementAction
{

    public float ChanceForForest;

    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        Debug.LogError("Random Chance was larger than 1.0, so higher than 100%. Chance was: " + ChanceForForest);
        if( Random.value <= ChanceForForest)
        {
            //TODO ANNA
            //Grant a forest
        }
        else
        {
            //Grant a settlement
        }


    }
}
