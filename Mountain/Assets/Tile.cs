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

    private TileGridLayout Map => transform.parent.GetComponent<TileGridLayout>();

    private void OnMouseEnter()
        => GetComponentInParent<TileGridLayout>().OnMouseEnterTile(this);

    private void OnMouseDown()
        => GetComponentInParent<TileGridLayout>().OnMouseDownTile(this);

}
