using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int Location {get; set; }

    public List<Entity> Entities;
    public TerrainType Terrain = TerrainType.Plains;
    //public Card Card;

    public enum TerrainType
    {
        None,
        Plains,
        Forest,
        Mountain,
        MountainTunnel,
        Quest,
        Goal,
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
