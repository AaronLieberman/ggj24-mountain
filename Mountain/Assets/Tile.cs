using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int Location { get; set; }

    public Placement SpawnPlacement(Placement placement)
    {
        Utilities.DestroyAllChildren(transform);

        return Instantiate(placement, transform);
    }

    private TileGridLayout Map => transform.parent.GetComponent<TileGridLayout>();

    private void OnMouseEnter()
        => Utilities.GetRootComponent<GameManager>().OnMouseEnterTile(this);

    private void OnMouseDown()
        => Utilities.GetRootComponent<GameManager>().OnMouseDownTile(this);

    private void OnMouseUp()
        => GetComponentInParent<TileGridLayout>().OnMouseUpTile(this);

    private void OnMouseOver()
        => Utilities.GetRootComponent<GameManager>().OnMouseOverTile(this);

    public void SetHighlight(bool value)
    {
    }
}
