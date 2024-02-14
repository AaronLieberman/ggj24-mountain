using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContagiousMangroveAction : PlacementAction
{
    public float ChanceToSwampify = 30;
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if (Random.Range(0.0f, 100.0f) <= ChanceToSwampify)
        {
            if (worker.WorkerPlans.Count > 0)
            {
                int randomIndex = Random.Range(0, worker.WorkerPlans.Count);
                PutOnDeck(worker.WorkerPlans[randomIndex].Card.PlacementToSpawn);

                PlacementPoolManager ppm = Utilities.GetRootComponent<PlacementPoolManager>();
                Placement newSwamp = ppm.GetRandomCardFromBiome(Utilities.GetRootComponent<Deck>().SwampBiome);

                worker.ReplaceDestination(randomIndex, newSwamp);
            }
        }
    }

    protected void PutOnDeck(Placement cardToAdd)
    {
        Utilities.GetRootComponent<Deck>().AddNewCardToDeck(cardToAdd, true);
    }

}
