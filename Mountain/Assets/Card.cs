using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TileSpawnDesc
{
    public Tile Tile;
    public float Weight = 1;
}

public class Card : MonoBehaviour
{
    public List<TileSpawnDesc> TileSpawn;

    Unity.Mathematics.Random _random = new();

    public void SpawnAtLocation(Tile tile)
    {
        Utilities.DestroyAllChildren(tile.transform);

        var chosenTile = ChooseTileToSpawn();
        if (chosenTile == null)
            return;

        Instantiate(chosenTile, tile.transform);
    }

    Tile ChooseTileToSpawn()
    {
        var totalWeight = TileSpawn.Sum(a => a.Weight);
        if (totalWeight <= 0)
            return null;

        var target = _random.NextFloat(totalWeight);

        float weightSoFar = 0;
        foreach (var spawn in TileSpawn)
        {
            weightSoFar += spawn.Weight;
            if (target < weightSoFar)
                return spawn.Tile;
        }

        return null;
    }
}
