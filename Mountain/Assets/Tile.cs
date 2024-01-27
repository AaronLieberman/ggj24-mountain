using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int Location { get; set; }

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

    public void SpawnPlacement(Placement placement)
    {
        Utilities.DestroyAllChildren(transform);

        Instantiate(placement, transform);
    }

    void OnMouseDown()
    {
        Debug.Log(gameObject.name + " was clicked.");
    }
}
