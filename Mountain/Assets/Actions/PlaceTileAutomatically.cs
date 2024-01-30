using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTileAutomatically : PlacementAction
{
    public Placement tileToPlace;
    public Vector2Int placementCoordinates;

    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if (tileToPlace == null)
            return;

        var map = Utilities.GetRootComponent<TileGridLayout>();
        map.GetTileFromLoc(placementCoordinates).SpawnPlacement(tileToPlace);
    }
}
