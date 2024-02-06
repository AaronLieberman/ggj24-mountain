using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RemoveTileFromJourneyPlan : PlacementAction
{
    public bool RemoveNextTile = false;
    public bool RemoveAllTiles = false;
    public bool RemoveRandomTile = false;
    public bool RemoveLastTile = false;

    public bool ShouldPutOnTopOfDeck = false;

    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if (RemoveNextTile && worker.WorkerPlans.Count > 0) 
        { 
            RemoveTile(worker.WorkerPlans.First()); 
        }

        if (RemoveAllTiles)
        {
            foreach (WorkerPlan pl in worker.WorkerPlans)
            {
                RemoveTile(pl);
            }
        }

        if (RemoveRandomTile && worker.WorkerPlans.Count > 0)
        {
            int randomIndex = Random.Range(0, worker.WorkerPlans.Count);
            RemoveTile(worker.WorkerPlans[randomIndex]);
        }

        if (RemoveLastTile && worker.WorkerPlans.Count > 0)
        {
            RemoveTile(worker.WorkerPlans.Last());
        }

    }

    public void RemoveTile(WorkerPlan plan)
    {
        if (ShouldPutOnTopOfDeck) PutOnDeck(plan);
        plan.Card = null;
    }

    protected void PutOnDeck(WorkerPlan cardToAdd)
    {
        Utilities.GetRootComponent<Deck>().AddNewCardToDeck(cardToAdd.Card.PlacementToSpawn, true);
    }

}
