using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utilities
{
	public static T GetRootComponent<T>() where T : Component
	{
		return SceneManager.GetActiveScene().GetRootGameObjects()
			.Select(a => a.GetComponent<T>())
			.Single(a => a != null);
	}

	public static IEnumerable<T> GetRootComponents<T>() where T : Component
	{
		return SceneManager.GetActiveScene().GetRootGameObjects()
			.SelectMany(a => a.GetComponents<T>());
	}

	public static T GetRootComponentRecursive<T>() where T : Component
	{
		return SceneManager.GetActiveScene().GetRootGameObjects()
			.Select(a => a.GetComponentInChildren<T>())
			.SingleOrDefault(a => a != null);
	}

	public static void DestroyAllChildren(GameObject go)
	{
		for (int i = go.transform.childCount - 1; i >= 0; --i)
		{
#if UNITY_EDITOR
			GameObject.DestroyImmediate(go.transform.GetChild(i).gameObject);
#else
			GameObject.Destroy(go.transform.GetChild(i).gameObject);
#endif
		}
	}

	public static void DestroyAllChildren(Transform transform)
	{
		foreach (Transform child in transform)
		{
			GameObject.Destroy(child.gameObject);
		}
	}

	public static IEnumerable<Transform> GetParents(this Transform transform)
	{
		var currentParent = transform.parent;
		while (currentParent != null)
		{
			yield return currentParent;
			currentParent = currentParent.parent;
		}
	}

	public static T FindParentWithComponent<T>(this Transform child) where T : Component
	{
		return child.GetParents()
			.Select(parent => parent.GetComponent<T>())
			.FirstOrDefault(component => component != null);
	}

	public static Vector2Int ToVec2I(Vector3Int v3i)
	{
		return new Vector2Int(v3i.x, v3i.y);
	}

	public static Vector3Int ToVec3I(Vector2Int v2i)
	{
		return new Vector3Int(v2i.x, v2i.y, 0);
	}

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

    public static IEnumerable<Vector2Int> GetAdjacentHexOffsets(Vector2Int coord)
    {
        return coord.y % 2 == 0
            ? _directionsEven
            : _directionsOdd;
    }

    public static IEnumerable<Vector2Int> GetAdjacentHexCoords(Vector2Int coord)
    {
        return coord.y % 2 == 0
            ? _directionsEven.Select(d => coord + d)
            : _directionsOdd.Select(d => coord + d);
    }

    public static List<Tile> GetAdjacentTiles(Vector2Int coord)
    {
        var map = GetRootComponent<TileGridLayout>();

        IEnumerable<Vector2Int> adjacentCoords = GetAdjacentHexCoords(coord);
		List<Tile> adjTiles = new List<Tile>();

		foreach (var adjCoord in adjacentCoords)
		{
            Tile tileForLoc = map.GetTileFromLoc(adjCoord);
			if (tileForLoc != null)
			{
				adjTiles.Add(tileForLoc);
			}
        }
		return adjTiles;
    }
}
