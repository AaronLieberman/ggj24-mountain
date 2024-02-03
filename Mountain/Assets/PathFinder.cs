using System.Collections.Generic;
using System.Linq;
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

    public static Dictionary<Vector2Int, int> CalculateDistances(TileGridLayout tileGridLayout, Vector2Int fromCoord, int maxDistance)
    {
        maxDistance = System.Math.Min(maxDistance, 200); //don't infinitely traverse empty space, bail out early so we don't loop to something like int.MaxValue

        var grid = tileGridLayout.GetComponent<Grid>();

        var results = new Dictionary<Vector2Int, int>();

        var stack = new Stack<(Vector2Int, int)>();
        stack.Push((fromCoord, 0));

        while (stack.Any())
        {
            var (current, distance) = stack.Pop();
            Debug.Assert(!results.ContainsKey(current));
            results[current] = distance;

            if (distance < maxDistance)
            {
                foreach (var adjacent in Utilities.GetAdjacentHexCoords(current).Where(a => !results.ContainsKey(a)))
                {
                    stack.Push((adjacent, distance + 1));
                }
            }
        }

        return results;
    }
}
