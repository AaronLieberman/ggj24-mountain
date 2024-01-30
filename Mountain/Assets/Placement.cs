using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileAction
{
    public string Cost;
    public Placement Upgrade;
    public PlacementAction OnUpgrade;
}

public class Placement : MonoBehaviour
{
    public List<TileAction> Actions;
    public string Name;
    public Sprite CardSprite;
    public string FlavorText;
    public string OnRevealText;
    public string OnVisitText;
    public float LostChance;
    public PlacementAction OnVisit;
    public int Difficulty;
    public float PathingHeuristic;  // Impassable > 10000
    public string PaysCost;
    public PlacementAction RevealAction;
}
