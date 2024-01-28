
using System.Collections.Generic;
using System;
using System.Linq;

public interface INeighborQueryable<T>
{
    IEnumerable<T> GetNeighbors();
    float GetHeuristic();
    float CalcDist(T other);
}

// https://en.wikipedia.org/wiki/A*_search_algorithm
public static class PathfinderAStar<T> where T : INeighborQueryable<T>
{
    private static List<T> ReconstructPath(Dictionary<T, T> cameFrom, T current)
    {
        var totalPath = new List<T> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current); 
        }
        return totalPath;
    }

    public static List<T> CalculateRoute(T start, T goal) 
    {
        var openSet = new HashSet<T> { start };
        var cameFrom = new Dictionary<T, T>();

        var minScore = new Dictionary<T, float>();
        minScore[start] = 0;

        var predScore = new Dictionary<T, float>();
        predScore[start] = start.GetHeuristic();

        while (openSet.Any())
        {
            var current = openSet.OrderBy(node => predScore.ContainsKey(node) ? predScore[node] : float.MaxValue).First();

            if (EqualityComparer<T>.Default.Equals(current, goal))
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (var neighbor in current.GetNeighbors())
            {
                float tentativeGScore = minScore[current] + current.CalcDist(neighbor);
                if (tentativeGScore < minScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    minScore[neighbor] = tentativeGScore;
                    predScore[neighbor] = tentativeGScore + neighbor.GetHeuristic();
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }
}
