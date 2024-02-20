using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TileStatus
{
    Hidden,
    Disabled,
    Highlighted,
    StaticallyImpassbile,
}

public class Tile : MonoBehaviour
{
    public Vector2Int Location { get; set; }

    Dictionary<TileStatus, HashSet<string>> _statusReasons = new();
    Material _highlightMaterial;
    Material _originalMaterial;
    bool _isRevealed = false;

    public Placement Placement => GetComponentInChildren<Placement>();
    public bool IsRevealed => _isRevealed;

    void Awake()
    {
        foreach (TileStatus ts in Enum.GetValues(typeof(TileStatus)))
        {
            _statusReasons[ts] = new HashSet<string>();
        }

        _highlightMaterial = transform.GetComponentInParent<TileGridLayout>().HighlightMaterial;
    }

    public Placement SpawnPlacement(Placement placement, bool isRevealed = true)
    {
        return SpawnPlacement(null, placement, isRevealed);
    }

    // worker may be null
    public Placement SpawnPlacement(Worker worker, Placement placement, bool isRevealed = false)
    {
        Utilities.DestroyAllChildren(transform);

        var instance = Instantiate(placement, transform, false);
        _isRevealed = isRevealed;

        if (!string.IsNullOrWhiteSpace(instance.OnRevealText))
        {
            Utilities.GetRootComponent<GameManager>().ShowOnRevealText(instance.OnRevealText);
        }

        if (instance.RevealAction != null)
        {
            instance.RevealAction.DoWork(worker, instance, null);
        }

        foreach (PlacementAction action in placement.RevealActions)
        {
            action.DoWork(worker, instance, null);
        }

        return instance;
    }

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

    public void SetStatus(TileStatus status, string reason, bool value)
    {
        var reasons = _statusReasons.GetValueOrDefault(status);
        if (reasons == null)
        {
            reasons = new HashSet<string>();
            _statusReasons[status] = reasons;
        }

        if (value && !reasons.Contains(reason))
        {
            reasons.Add(reason);
        }
        else if (!value && reasons.Contains(reason))
        {
            reasons.Remove(reason);
        }

        RefreshStatusEffects();
    }

    public bool GetStatus(TileStatus status)
    {
        return _statusReasons[status].Any();
    }

    public void RefreshStatusEffects()
    {
        var disabledReasons = _statusReasons[TileStatus.Disabled];
        var hiddenReasons = _statusReasons[TileStatus.Hidden];
        var highlightedReasons = _statusReasons[TileStatus.Highlighted];
        var staticallyImpassibleReasons = _statusReasons[TileStatus.StaticallyImpassbile];

        if (_originalMaterial == null)
        {
            _originalMaterial = transform.GetComponentsInChildren<SpriteRenderer>().Select(a => a.material).First();
        }

        foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            float opacity = 1;
            if (disabledReasons.Any())
            {
                opacity = 0.5f;
            }
            else if (staticallyImpassibleReasons.Any())
            {
                opacity = 0.75f;
            }

            sr.color = new Color(opacity, opacity, opacity);
            sr.enabled = !hiddenReasons.Any();
            sr.material = highlightedReasons.Any() ? _highlightMaterial : (_originalMaterial != null ? _originalMaterial : sr.material);
        }
    }
}

class TileNeighborQuierier : INeighborQuerier<Tile>
{
    public IEnumerable<Tile> GetNeighbors(Tile tile)
        => tile.GetComponentInParent<TileGridLayout>().GetNeighborsByTile(tile);

    public GetHeuristicResult GetHeuristic(Tile tile, bool isEnd)
    {
        if (isEnd) return new GetHeuristicResult { IsPassible = true, Weight = 0 };
        var placement = tile.GetComponentInChildren<Placement>();
        if (placement == null) return new GetHeuristicResult { IsPassible = true, Weight = 1 };

        return new GetHeuristicResult { IsPassible = placement.IsPassable, Weight = placement.PathingHeuristic };
    }

    public float CalcDist(Tile tile, Tile other)
        => Vector3.Distance(tile.transform.position, other.transform.position);
}
