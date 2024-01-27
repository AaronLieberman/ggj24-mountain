using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    readonly static Vector2Int[] _directionsEven = new Vector2Int[]
    {
        new (-1, 1), new (0, 1),
        new (-1, 0), new (1, 0),
        new (-1, -1), new (0, -1),
    };

    readonly static Vector2Int[] _directionsOdd = new Vector2Int[]
    {
        new (0, 1), new (1, 1), 
        new (-1, 0), new (1, 0),
        new (0, -1), new (1, -1),
    };

    public static IEnumerable<Vector2Int> GetAdjacentHexCoords(Vector2Int coord)
    {
        return coord.y % 2 == 0
            ? _directionsEven.Select(d => coord + d)
            : _directionsOdd.Select(d => coord + d);
    }

    static float GetDistance(Grid grid, Vector2Int a, Vector2Int b)
    {
        return Vector3.Distance(grid.GetCellCenterLocal(new Vector3Int(a.x, a.y, 0)), grid.GetCellCenterLocal(new Vector3Int(b.x, b.y, 0)));
    }

    public static IEnumerable<Vector2Int> CalculateRoute(TileGridLayout tileGridLayout, Vector2Int fromCoord, Vector2Int toCoord, int maxIterations = 100)
    {
        var grid = tileGridLayout.GetComponent<Grid>();

        var currentCoord = fromCoord;

        int iterations = 0;
        var results = new List<Vector2Int>();
        while (currentCoord != toCoord && iterations < maxIterations)
        {
            iterations++;

            var minDistance = float.MaxValue;
            Vector2Int closerCoord = toCoord;

            foreach (var coord in GetAdjacentHexCoords(currentCoord))
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
}
