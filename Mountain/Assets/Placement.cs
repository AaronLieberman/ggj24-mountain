using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileAction
{
    public Card Cost;
    public Tile Upgrade;
    public MonoBehaviour Action;
}

public class Placement : MonoBehaviour
{
    public List<TileAction> Actions;
    public string Name;
    public string FlavorText;
    public float LostChance;
    public int Difficulty;
    public float PathingHeuristic;  // 0.0 to 1.0
    public ICardReveal RevealAction;
}
