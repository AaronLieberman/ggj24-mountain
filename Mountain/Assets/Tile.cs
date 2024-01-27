using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int Location {get; set; }

    public List<Entity> Entities;
    public TerrainType Terrain = TerrainType.Plains;
    public RevealState Reveal = RevealState.Hidden;
    //public Card Card;

    public enum TerrainType
    {
        Plains,
        Forest,
        Mountain,
        MountainTunnel,
        Goal,
    }

    public enum RevealState
    {
        Visible,
        VisibleQuest,
        Hidden,
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
