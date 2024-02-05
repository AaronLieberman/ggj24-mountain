using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanopyVillageDeckingAbility : PlacementAction
{

    public float ChanceForWoods = 0.5f;

    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if(ChanceForWoods> 1.0f) Debug.LogError("Random Chance was larger than 1.0, so higher than 100%. Chance was: " + ChanceForWoods);
        if( Random.value <= ChanceForWoods)
        {
            //Grant a forest
            Utilities.GetRootComponent<PlacementPoolManager>().AddToDeckFromBiomeInPool(Utilities.GetRootComponent<Deck>().WoodsBiome, 1);
        }
        else
        {
            //Grant a settlement
            Utilities.GetRootComponent<PlacementPoolManager>().AddToDeckFromBiomeInPool(Utilities.GetRootComponent<Deck>().SettlementBiome, 1);
        }
    }
}
