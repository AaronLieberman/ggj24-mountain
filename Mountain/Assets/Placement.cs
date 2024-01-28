using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileAction
{
    public string Cost;
    public Placement Upgrade;
    public IUpgradeAction Action;
}

public class Placement : MonoBehaviour
{
    public List<TileAction> Actions;
    public string Name;
    public string FlavorText;
    public string OnRevealText;
    public string OnVisitText;
    public float LostChance;
    public int Difficulty;
    public float PathingHeuristic;  // 0.0 to 1.0
    public string PaysCost;
    public ICardReveal RevealAction;
}
