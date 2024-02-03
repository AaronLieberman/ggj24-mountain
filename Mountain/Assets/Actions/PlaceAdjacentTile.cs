using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlaceAdjacentTile : PlacementAction
{
    public Placement TileToPlace;
    public bool UseThisPlacementInsteadOfCenterCoordinates = true;
    public Vector2Int CenterCoordinates;
    public int MinimumDistanceByAdjacency = 1;
    public int MaximumDistanceByAdjacency = int.MaxValue;
    public int MaximumDistanceByPathing = 100000;
    public bool MustBeUnexplored = true;

    protected Dictionary<Vector2Int, int> calculatedDistances;

    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if (TileToPlace == null)
            return;

        if (UseThisPlacementInsteadOfCenterCoordinates)
        {
            CenterCoordinates = placement.GetComponentInParent<Tile>().Location;
        }

        calculatedDistances = PathFinder.CalculateDistances(Utilities.GetRootComponent<TileGridLayout>(), CenterCoordinates, MaximumDistanceByAdjacency);

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

            List<Tile> adjTiles = Utilities.GetAdjacentTiles(thisTile.Location);
            //Add any of the adjacent tiles that we haven't already visited to the tilesToVisit:
            foreach (Tile t in adjTiles.Where(tile => (!tilesAlreadyVisited.Contains(tile) && !tilesToVisit.Contains(tile)) && calculatedDistances[tile.Location] < MaximumDistanceByAdjacency)) { 
                tilesToVisit.Enqueue(t);
            }
        }

        return validTiles;

    }

    private bool TileIsAValidPlacement(Tile tileToVisit)
    {

        // If we only want unexplored tiles, and this tile is unexplored. Or we don't care.
        if (( MustBeUnexplored && tileToVisit.Placement.Name != Utilities.GetRootComponent<TileGridLayout>().DefaultPrefab.Name )) return false;

        Tile originTile = Utilities.GetRootComponent<TileGridLayout>().GetTileFromLoc(CenterCoordinates);

        //Make sure we're within the adjacency distance
        int distanceByAdjacency = calculatedDistances[tileToVisit.Location];
        if (distanceByAdjacency < MinimumDistanceByAdjacency) return false;
        if (distanceByAdjacency > MaximumDistanceByAdjacency) return false;

        //Make sure we are close enough by pathfinding
        int lengthOfPath = PathfinderAStar<Tile>.CalculateRoute(originTile, tileToVisit).Count;
        if (lengthOfPath > MaximumDistanceByPathing) return false;
        
        // We made it past all the checks!
        return true;
    }
}
