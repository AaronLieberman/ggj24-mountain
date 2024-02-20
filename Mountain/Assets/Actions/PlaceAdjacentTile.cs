using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceAdjacentTile : PlacementAction
{
    public Placement TileToPlace;
    public bool UseThisPlacementInsteadOfCenterCoordinates = true;
    public Vector2Int CenterCoordinates;
    public int MinimumDistanceByAdjacency = 1;
    public int MaximumDistanceByAdjacency = int.MaxValue;
    public int MaximumDistanceByPathing = -1;
    public bool MustBeUnexplored = true;

    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        if (TileToPlace == null)
            return;

        if (UseThisPlacementInsteadOfCenterCoordinates)
        {
            CenterCoordinates = placement.GetComponentInParent<Tile>().Location;
        }

        List<Tile> validTiles = GetValidTiles(placement);

        if (validTiles.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, validTiles.Count);
            validTiles[randomIndex].SpawnPlacement(TileToPlace);
        }
        else
        {
            Debug.Log("No valid tiles were found while placing adjacent.");
        }
    }

    protected List<Tile> GetValidTiles(Placement placement)
    {
        Tile originTile = placement.GetComponentInParent<Tile>();
        List<Tile> validTiles = new();
        Queue<Tile> tilesToVisit = new();
        HashSet<Tile> tilesAlreadyVisited = new();

        tilesToVisit.Enqueue(originTile);

        while (tilesToVisit.Count > 0)
        {
            Tile thisTile = tilesToVisit.Dequeue();
            tilesAlreadyVisited.Add(thisTile);
            if (TileIsAValidPlacement(thisTile))
            {
                validTiles.Add(thisTile);
            }

            List<Tile> adjTiles = Utilities.GetAdjacentTiles(thisTile.Location);
            //Add any of the adjacent tiles that we haven't already visited to the tilesToVisit:
            foreach (Tile t in adjTiles.Where(tile => !tilesAlreadyVisited.Contains(tile) && !tilesToVisit.Contains(tile) && PathFinder.CalculateDistanceDirect(originTile, tile) <= MaximumDistanceByAdjacency))
            {
                tilesToVisit.Enqueue(t);
            }
        }

        return validTiles;

    }

    private bool TileIsAValidPlacement(Tile tileToVisit)
    {
        // If we only want unexplored tiles, and this tile is unexplored. Or we don't care.
        if (MustBeUnexplored && tileToVisit.Placement.Name != Utilities.GetRootComponent<TileGridLayout>().DefaultPrefab.Name) return false;

        //Make sure we're within the adjacency distance
        int distanceByAdjacency = PathFinder.CalculateDistanceDirect(CenterCoordinates, tileToVisit.Location);
        if (distanceByAdjacency < MinimumDistanceByAdjacency) return false;
        if (distanceByAdjacency > MaximumDistanceByAdjacency) return false;

        if (MaximumDistanceByPathing > 0)
        {
            //Make sure we are close enough by pathfinding
            Tile originTile = Utilities.GetRootComponent<TileGridLayout>().GetTileFromLoc(CenterCoordinates);
            List<Tile> routeToTile = TilePathfinderAStar.CalculateRoute(originTile, tileToVisit);
            if (routeToTile == null || routeToTile.Count > MaximumDistanceByPathing + 1) return false; //The Count includes the origin, so we need to add a + 1
        }

        // We made it past all the checks!
        return true;
    }
}
