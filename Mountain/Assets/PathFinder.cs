using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

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

    public static int CalculateDistanceDirect(Vector2Int p1, Vector2Int p2)
    {
        bool isEven(int v) => v % 2 == 0;
        bool isOdd(int v) => v % 2 == 1;
        var dx = p2.x - p1.x;
        var dy = p2.y - p1.y;

        var penalty = ((isEven(p1.y) && isOdd(p2.y) && (p1.x < p2.x)) || (isEven(p2.y) && isOdd(p1.y) && (p2.x < p1.x))) ? 1 : 0;
        return math.max(math.abs(dy), math.abs(dx) + math.abs(dy) / 2 + penalty);
    }

    public static Dictionary<Vector2Int, int> CalculateDistances(Vector2Int fromCoord, int maxDistance)
    {
        // don't infinitely traverse empty space, bail out early so we don't loop to something like int.MaxValue
        maxDistance = Math.Min(maxDistance, 200);

        var results = new Dictionary<Vector2Int, int>();

        var queue = new Queue<(Vector2Int, int)>();
        var seen = new HashSet<Vector2Int>() { fromCoord };
        queue.Enqueue((fromCoord, 0));
        int directDistance = CalculateDistanceDirect(fromCoord, fromCoord);
        Debug.Assert(directDistance == 0, $"direct={directDistance}, distance={0}");

        while (queue.Any())
        {
            var (current, distance) = queue.Dequeue();
            Debug.Assert(!results.ContainsKey(current), $"already contains {current}");
            results[current] = distance;
            directDistance = CalculateDistanceDirect(fromCoord, current);
            Debug.Assert(directDistance == distance, $"from={fromCoord}, to={current}, direct={directDistance}, distance={distance}");

            if (distance < maxDistance)
            {
                foreach (var adjacent in Utilities.GetAdjacentHexCoords(current).Where(a => !seen.Contains(a)))
                {
                    seen.Add(adjacent);
                    queue.Enqueue((adjacent, distance + 1));
                }
            }
        }

        return results;
    }
}
