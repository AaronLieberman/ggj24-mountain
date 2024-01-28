using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TileSpawnDesc
{
    public Placement Tile;
    public float Weight = 1;
}

[Serializable]
public class CardDetails
{
    public Sprite CardSprite;
}

public class Card : MonoBehaviour
{
    public List<TileSpawnDesc> TileSpawn;

    public CardDetails CardDetails;

    Unity.Mathematics.Random _random;
    void Awake()
    {
        _random = new Unity.Mathematics.Random(42);
    }

    public void SpawnOnTile(Tile tile)
    {
        tile.SpawnPlacement(ChoosePlacementToSpawn());
    }

    Placement ChoosePlacementToSpawn()
    {
        if (!TileSpawn.Any())
            return null;

        var totalWeight = TileSpawn.Sum(a => a.Weight);
        if (totalWeight <= 0)
            return TileSpawn[_random.NextInt(TileSpawn.Count)].Tile;

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
