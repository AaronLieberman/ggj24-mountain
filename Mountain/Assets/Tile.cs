using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, INeighborQueryable<Tile>
{
    public Vector2Int Location { get; set; }

    Material _highlightMaterial;
    Material _originalMaterial;

    void Awake()
    {
        _highlightMaterial = transform.GetComponentInParent<TileGridLayout>().HighlightMaterial;
    }

    public Placement SpawnPlacement(Placement placement)
    {
        Utilities.DestroyAllChildren(transform);

        return Instantiate(placement, transform);
    }

    public IEnumerable<Tile> GetNeighbors()
        => GetComponentInParent<TileGridLayout>().GetNeighborsByTile(this);

    
    public float GetHeuristic()
        => GetComponentInChildren<Placement>()?.PathingHeuristic ?? 1f;

    public float CalcDist(Tile other)
        => Vector3.Distance(transform.position, other.transform.position);

    private TileGridLayout Map => transform.parent.GetComponent<TileGridLayout>();

    private void OnMouseEnter()
        => Utilities.GetRootComponent<GameManager>().OnMouseEnterTile(this);

    private void OnMouseExit()
        => Utilities.GetRootComponent<GameManager>().OnMouseExitTile(this);

    private void OnMouseDown()
        => Utilities.GetRootComponent<GameManager>().OnMouseDownTile(this);

    private void OnMouseUp()
        => GetComponentInParent<TileGridLayout>().OnMouseUpTile(this);

    private void OnMouseOver()
        => Utilities.GetRootComponent<GameManager>().OnMouseOverTile(this);

    public void SetHighlight(bool value)
    {
        if (!value)
        {
            if (_originalMaterial != null)
            {
                foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
                {
                    sr.material = _originalMaterial;
                }
            }
        }
        else
        {
            foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                if (sr.material != _highlightMaterial)
                {
                    _originalMaterial = sr.material;
                }

                sr.material = _highlightMaterial;
            }
        }
    }
}
