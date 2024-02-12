using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class TileAction
{
    [Tooltip("If cost is set to None, then any card can be used to pay the cost")]
    public Placement Cost;
    public Placement Upgrade;
    public PlacementAction OnUpgrade;
	public bool SkipOnVisit;

    public bool CanPayCost(Card card)
    {
        // if you don't have a card then you can't use it to pay any cost
        if (card == null || card.PlacementToSpawn == null)
            return false;

        // cost is free, any card can pay it
        if (Cost == null)
            return true;

        // if the card is hidden, compare the cost to the hidden placement
        if (!card.IsRevealed)
            return card.PlacementToSpawn.Biome != null && card.PlacementToSpawn.Biome.Name == Cost.Name;

        // if the card is revealed, compare the cost to the revealed placement
        if (card.PlacementToSpawn.Name == Cost.Name)
            return true;

        // if the card is revealed, you can also pay using the placement's biome
        return card.PlacementToSpawn.Biome != null && card.PlacementToSpawn.Biome.Name == Cost.Name;
    }
}

public class Placement : MonoBehaviour
{
    public string Name;
    public Sprite CardSprite;
    public int Difficulty;
    public float LostChance;    
    public float PathingHeuristic;  // Impassable >= 10000
    public string FlavorText;
    public Placement Biome;
    public string OnRevealText;
    public PlacementAction RevealAction;
    public List<PlacementAction> RevealActions;
    public string OnVisitTooltipText;
    [FormerlySerializedAs("OnVisit")]
    public PlacementAction OnVisitAction;
    public List<PlacementAction> VisitActions;
    public ParticleSystem VisitationParticleSystem;
    public List<TileAction> Actions;

    public void Visited(Worker worker, Placement placement)
    {
        placement.OnVisitAction?.DoWork(worker, placement, null);
        foreach (PlacementAction action in placement.VisitActions)
        {
            action.DoWork(worker, placement, null);
        }
        if(VisitationParticleSystem) VisitationParticleSystem.Play();
    }
}
