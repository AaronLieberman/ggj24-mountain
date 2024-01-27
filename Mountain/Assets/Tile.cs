using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileAction
{
    public Card Cost;
    public Tile Upgrade;
    public MonoBehaviour Action;
}

public class Tile : MonoBehaviour
{
    public List<TileAction> Actions;
    public string FlavorText;

    public Vector2Int Location {get; set; }

    // public TerrainType Terrain = TerrainType.Plains;
    // //public Card Card;

    // public enum TerrainType
    // {
    //     None,
    //     Plains,
    //     Forest,
    //     Mountain,
    //     MountainTunnel,
    //     Quest,
    //     Goal,
    // }
}
