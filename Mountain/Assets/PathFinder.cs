using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public struct Passibility
{
    public bool Passable;
    public int UnexploredDistance;
}

public class PathFinder
{
    static float GetDistance(Grid grid, Vector2Int a, Vector2Int b)
    {
        return Vector3.Distance(grid.GetCellCenterLocal(new Vector3Int(a.x, a.y, 0)), grid.GetCellCenterLocal(new Vector3Int(b.x, b.y, 0)));
    }

    public static IEnumerable<Vector2Int> CalculateRoute(TileGridLayout tileGridLayout, Vector2Int fromCoord, Vector2Int toCoord, int maxIterations = 100)
    {
        var grid = tileGridLayout.GetComponent<Grid>();

        var currentCoord = fromCoord;

        int iterations = 0;
        var results = new List<Vector2Int> { fromCoord };
        while (currentCoord != toCoord && iterations < maxIterations)
        {
            iterations++;

            var minDistance = float.MaxValue;
            Vector2Int closerCoord = toCoord;

            foreach (var coord in Utilities.GetAdjacentHexCoords(currentCoord))
            {
                var d = GetDistance(grid, coord, toCoord);
                if (d < minDistance)
                {
                    minDistance = d;
                    closerCoord = coord;
                }
            }

            currentCoord = closerCoord;
            results.Add(currentCoord);
        }

        return results;
    }

    public static int CalculateDistanceDirect(Tile p1, Tile p2)
    {
        return CalculateDistanceDirect(p1.Location, p2.Location);
    }

    public static int CalculateDistanceDirect(Vector2Int p1, Vector2Int p2)
    {
        bool isEven(int v) => v % 2 == 0;
        bool isOdd(int v) => v % 2 == 1;
        var dx = p2.x - p1.x;
        var dy = p2.y - p1.y;

        var penalty = ((isEven(p1.y) && isOdd(p2.y) && (p1.x < p2.x)) || (isEven(p2.y) && isOdd(p1.y) && (p2.x < p1.x))) ? 1 : 0;
        return math.max(math.abs(dy), math.abs(dx) + math.abs(dy) / 2 + penalty);
    }

    public static Dictionary<Vector2Int, Passibility> CalculateUnexploredDistance(Vector2Int fromCoord, int maxUnexplored)
    {
        var grid = Utilities.GetRootComponent<TileGridLayout>();
        var unexploredPrefabName = grid.DefaultPrefab.Name;

        var results = new Dictionary<Vector2Int, Passibility>();
        results[fromCoord] = new Passibility { Passable = true, UnexploredDistance = 0 };

        var queue = new Queue<(Vector2Int, int)>();
        queue.Enqueue((fromCoord, 0));

        while (queue.Any())
        {
            var (current, distance) = queue.Dequeue();

            foreach (var adjacent in Utilities.GetAdjacentHexCoords(current))
            {
                var tile = grid.GetTileFromLoc(adjacent);
                if (tile == null)
                    continue;

                var adjacentDistance = distance;
                var placement = tile.GetComponentInChildren<Placement>();
                bool isUnexplored = placement.Name == unexploredPrefabName;
                if (isUnexplored)
                {
                    adjacentDistance++;
                }
                else if (placement.PathingHeuristic >= 10000)
                {
                    results[adjacent] = new Passibility { UnexploredDistance = -1 };
                    continue;
                }

                if (!results.ContainsKey(adjacent) || adjacentDistance < results[adjacent].UnexploredDistance)
                {
                    results[adjacent] = results[adjacent] = new Passibility { Passable = true, UnexploredDistance = adjacentDistance };

                    if (adjacentDistance < maxUnexplored)
                    {
                        queue.Enqueue((adjacent, adjacentDistance));
                    }
                }
            }
        }

        return results;
    }
}
