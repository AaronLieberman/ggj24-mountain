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

    public string PaysCost { get { return (IsRevealed ? PlacementToSpawn : UnrevealedPlacement).PaysCost; } }
}
