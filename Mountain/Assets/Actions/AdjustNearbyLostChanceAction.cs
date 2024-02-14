using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AdjustNearbyLostChanceAction : PlacementAction
{
    public float adjustmentToAdjacentLostChances;
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        // TODO: Make it so this can adjust an adjacency greater than 1
        List<Tile> adjacentTiles = Utilities.GetAdjacentTiles(placement.GetComponentInParent<Tile>().Location);

        foreach (Tile tile in adjacentTiles)
        {
            tile.Placement.LostChance += adjustmentToAdjacentLostChances;
        }
    }
}
