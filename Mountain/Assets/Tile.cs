using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public Vector2Int Location { get; set; }

    HashSet<string> _highlightReasons = new();
    HashSet<string> _disabledReasons = new();
    Material _highlightMaterial;
    Material _originalMaterial;

    public Placement Placement => GetComponentInChildren<Placement>();

    void Awake()
    {
        _highlightMaterial = transform.GetComponentInParent<TileGridLayout>().HighlightMaterial;
    }

    public Placement SpawnPlacement(Placement placement)
    {
        return SpawnPlacement(null, placement);
    }

    // worker may be null
    public Placement SpawnPlacement(Worker worker, Placement placement)
    {
        Utilities.DestroyAllChildren(transform);

        var instance = Instantiate(placement, transform, false);

        if (!string.IsNullOrWhiteSpace(instance.OnRevealText))
        {
            Utilities.GetRootComponent<GameManager>().ShowOnRevealText(instance.OnRevealText);
        }

        instance.RevealAction?.DoWork(worker, instance, null);
        return instance;
    }

    private TileGridLayout Map => transform.parent.GetComponent<TileGridLayout>();

    private bool ShouldHandleMouseEvents => !EventSystem.current.IsPointerOverGameObject();

    private void OnMouseEnter()
    {
        if (ShouldHandleMouseEvents)
            Utilities.GetRootComponent<GameManager>().OnMouseEnterTile(this);
    }

    private void OnMouseExit()
    {
        if (ShouldHandleMouseEvents)
            Utilities.GetRootComponent<GameManager>().OnMouseExitTile(this);
    }

    private void OnMouseUp()
    {
        if (ShouldHandleMouseEvents)
            Utilities.GetRootComponent<TileGridLayout>().OnMouseUpTile(this);
    }

    public void SetHighlight(string highlightReason, bool value)
    {
        if (value && !_highlightReasons.Contains(highlightReason))
        {
            _highlightReasons.Add(highlightReason);
        }
        else if (!value && _highlightReasons.Contains(highlightReason))
        {
            _highlightReasons.Remove(highlightReason);
        }

        if (_highlightReasons.Any())
        {
            if (_originalMaterial == null)
            {
                _originalMaterial = transform.GetComponentsInChildren<SpriteRenderer>().Select(a => a.material).First();
            }

            foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.material = _highlightMaterial;
            }
        }
        else
        {
            if (_originalMaterial != null)
            {
                foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
                {
                    sr.material = _originalMaterial;
                }
            }
        }
    }

    public void SetDisabled(string disabledReason, bool value)
    {
        if (value && !_disabledReasons.Contains(disabledReason))
        {
            _disabledReasons.Add(disabledReason);
        }
        else if (!value && _disabledReasons.Contains(disabledReason))
        {
            _disabledReasons.Remove(disabledReason);
        }

        if (_disabledReasons.Any())
        {
            float s = 0.5f;
            foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = new Color(s, s, s);
            }
        }
        else
        {
            foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.white;
            }
        }
    }
}

class TileNeighborQuierier : INeighborQuerier<Tile>
{
    public IEnumerable<Tile> GetNeighbors(Tile tile)
        => tile.GetComponentInParent<TileGridLayout>().GetNeighborsByTile(tile);

    public float GetHeuristic(Tile tile, bool isEnd)
        => isEnd ? 0 : tile.GetComponentInChildren<Placement>()?.PathingHeuristic ?? 1f;

    public float CalcDist(Tile tile, Tile other)
        => Vector3.Distance(tile.transform.position, other.transform.position);
}
