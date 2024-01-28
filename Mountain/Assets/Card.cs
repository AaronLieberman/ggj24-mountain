using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardDetails
{
    public Sprite CardSprite;
}

public class Card : MonoBehaviour
{
    public Placement PlacementToSpawn;
    public bool IsRevealed;

    public CardDetails CardDetails;

    Unity.Mathematics.Random _random;
    void Awake()
    {
        _random = new Unity.Mathematics.Random(42);
    }

    public void SpawnOnTile(Tile tile)
    {
        if (PlacementToSpawn != null)
        {
            tile.SpawnPlacement(PlacementToSpawn);
        }
    }
}
