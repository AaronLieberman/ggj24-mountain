using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTileAutomatically : PlacementAction
{
    public Placement tileToPlace;
    public Vector2Int placementCoordinates;
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        var map = Utilities.GetRootComponent<TileGridLayout>();
        map.GetTileFromLoc(placementCoordinates).SpawnPlacement(tileToPlace);
    }

}
