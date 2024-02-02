using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlaceAdjacentTile : PlacementAction
{
    public Placement TileToPlace;
    public bool useThisPlacementAsCenterCoordinates;
    public Vector2Int CenterCoordinates;
    public int MinimumDistanceByAdjacency = 1;
    public int MaximumDistanceByAdjacency = int.MaxValue;
    public int MaximumDistanceByPathing = 100000;
    public bool MustBeUnexplored = true;


    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if (TileToPlace == null)
            return;

        List<Tile> validTiles = GetValidTiles(placement);

        int randomIndex = UnityEngine.Random.Range(0, validTiles.Count);
        validTiles[randomIndex].SpawnPlacement(TileToPlace);
    }

    protected List<Tile> GetValidTiles(Placement placement)
    {
        List<Tile> validTiles = new List<Tile>();
        Queue<Tile> tilesToVisit = new Queue<Tile>();
        HashSet<Tile> tilesAlreadyVisited = new HashSet<Tile>();

        tilesToVisit.Enqueue(placement.GetComponentInParent<Tile>());

        while(tilesToVisit.Count > 0)
        {
            Tile thisTile = tilesToVisit.Dequeue();
            tilesAlreadyVisited.Add(thisTile);
            if (TileIsAValidPlacement(thisTile))
            {
                validTiles.Add(thisTile);
            }

            List<Tile> adjTiles = Utilities.GetAdjacentTiles(thisTile.GetComponentInParent<Tile>().Location);
            //Add any of the adjacent tiles that we haven't already visited to the tilesToVisit:
            foreach(Tile t in adjTiles.Where(tile => !tilesAlreadyVisited.Contains(tile) && !tilesToVisit.Contains(tile))) { 
                tilesToVisit.Enqueue(t);
            }
        }

        return validTiles;

    }

    private bool TileIsAValidPlacement(Tile tileToVisit)
    {

        // If we only want unexplored tiles, and this tile is unexplored. Or we don't care.
        if (( MustBeUnexplored && tileToVisit.Placement.Name == Utilities.GetRootComponent<TileGridLayout>().DefaultPrefab.Name ) || !MustBeUnexplored)
        {
            //public int MaximumDistanceByPathing = 100000;

            //TODO @AARON
            if(MinimumDistanceByAdjacency <= 1 && 1 <= MaximumDistanceByAdjacency)
            {
            
                if(PathfinderAStar<Tile>.CalculateRoute(tileToVisit, placement.GetComponentInParent<Tile>()))
                return true;
            }
            
        }
        return false;
    }
}
