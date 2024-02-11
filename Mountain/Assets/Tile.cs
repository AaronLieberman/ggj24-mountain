using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public Vector2Int Location { get; set; }

    HashSet<string> _highlightReasons = new();
    HashSet<string> _disabledReasons = new();
    HashSet<string> _hiddenReason = new();
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
        foreach(PlacementAction action in placement.RevealActions)
        {
            action.DoWork(worker, instance, null);
        }
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

    public void SetHighlight(string reason, bool value)
    {
        if (value && !_highlightReasons.Contains(reason))
        {
            _highlightReasons.Add(reason);
        }
        else if (!value && _highlightReasons.Contains(reason))
        {
            _highlightReasons.Remove(reason);
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

    public void SetDisabled(string reason, bool value)
    {
        if (value && !_disabledReasons.Contains(reason))
        {
            _disabledReasons.Add(reason);
        }
        else if (!value && _disabledReasons.Contains(reason))
        {
            _disabledReasons.Remove(reason);
        }

        float s = 0.5f;

        foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = _disabledReasons.Any() ? new Color(s, s, s) : Color.white;
        }
    }

    public void SetHidden(string reason, bool value)
    {
        if (value && !_hiddenReason.Contains(reason))
        {
            _hiddenReason.Add(reason);
        }
        else if (!value && _hiddenReason.Contains(reason))
        {
            _hiddenReason.Remove(reason);
        }

        foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = !_hiddenReason.Any();
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
