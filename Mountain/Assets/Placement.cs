using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class TileAction
{
    public Placement Cost;
    public Placement Upgrade;
    public PlacementAction OnUpgrade;

    public bool CanPayCost(Card card)
    {
        // cost is free, any card can pay it
        if (Cost == null)
            return true;

        // if the card is hidden, compare the cost to the hidden placement
        if (!card.IsRevealed)
            return card.UnrevealedPlacement != null && card.UnrevealedPlacement.Name == Cost.Name;

        // if the card is revealed, compare the cost to the revealed placement
        if (card.PlacementToSpawn != null && card.PlacementToSpawn.Name == Cost.Name)
            return true;

        // if the card is revealed, you can also pay using the unrevealed (base biome) placement
        return card.UnrevealedPlacement != null && card.UnrevealedPlacement.Name == Cost.Name;
    }
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
    public Placement PaysCost;
    public PlacementAction RevealAction;
}
