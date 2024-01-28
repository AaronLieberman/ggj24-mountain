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
    public Placement UnrevealedPlacement;
    public bool IsRevealed;

    public CardDetails CardDetails;

    public void SpawnOnTile(Tile tile)
    {
        if (PlacementToSpawn != null)
        {
            tile.SpawnPlacement(PlacementToSpawn);
        }
    }
}
