
using System.Collections.Generic;
using System;
using System.Linq;

public struct GetHeuristicResult
{
    public  bool IsPassible;
    public  float Weight;
}

public interface INeighborQuerier<T>
{
    IEnumerable<T> GetNeighbors(T t);
    GetHeuristicResult GetHeuristic(T t, bool isEnd);
    float CalcDist(T t, T other);
}

// https://en.wikipedia.org/wiki/A*_search_algorithm
public static class PathfinderAStar
{
    static List<T> ReconstructPath<T>(Dictionary<T, T> cameFrom, T current)
    {
        var totalPath = new List<T> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    public static List<T> CalculateRoute<T>(INeighborQuerier<T> querier, T start, T goal)
    {
        var openSet = new HashSet<T> { start };
        var cameFrom = new Dictionary<T, T>();

        var minScore = new Dictionary<T, float>();
        minScore[start] = 0;

        var predScore = new Dictionary<T, float>();
        var result = querier.GetHeuristic(start, start.Equals(goal));
        predScore[start] = result.Weight;

        while (openSet.Any())
        {
            var current = openSet.OrderBy(node => predScore.ContainsKey(node) ? predScore[node] : float.MaxValue).First();

            if (EqualityComparer<T>.Default.Equals(current, goal))
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (var neighbor in querier.GetNeighbors(current))
            {
                result = querier.GetHeuristic(neighbor, neighbor.Equals(goal));
                if (!result.IsPassible)
                    continue;

                float tentativeGScore = minScore[current] + querier.CalcDist(current, neighbor);
                if (tentativeGScore < minScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    minScore[neighbor] = tentativeGScore;
                    predScore[neighbor] = tentativeGScore + result.Weight;
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }
}

public static class TilePathfinderAStar
{
    static readonly TileNeighborQuierier _quierier = new();

    public static List<Tile> CalculateRoute(Tile start, Tile end)
    {
        return PathfinderAStar.CalculateRoute(_quierier, start, end);
    }
}
