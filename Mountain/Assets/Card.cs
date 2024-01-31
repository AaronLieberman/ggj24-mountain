using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Placement PlacementToSpawn;
    public Placement UnrevealedPlacement;
    public bool IsRevealed;

    public Placement PaysCost { get { return (IsRevealed ? PlacementToSpawn : UnrevealedPlacement)?.PaysCost ?? null; } }
}
