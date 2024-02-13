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
                RemoveTile(worker.WorkerPlans[randomIndex]);
            }
        }
    }

    public void RemoveTile(WorkerPlan plan)
    {
        PutOnDeck(plan);
        Destroy(plan.Card);
        plan.Card = null;
    }

    protected void PutOnDeck(WorkerPlan cardToAdd)
    {
        Utilities.GetRootComponent<Deck>().AddNewCardToDeck(cardToAdd.Card.PlacementToSpawn, true);
    }

}
