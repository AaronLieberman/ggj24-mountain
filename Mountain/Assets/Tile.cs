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
}

public class Tile : MonoBehaviour
{
    public Vector2Int Location { get; set; }

    Dictionary<TileStatus, HashSet<string>> _statusReasons = new();
    Material _highlightMaterial;
    Material _originalMaterial;

    public Placement Placement => GetComponentInChildren<Placement>();

    void Awake()
    {
        _statusReasons[TileStatus.Disabled] = new HashSet<string>();
        _statusReasons[TileStatus.Hidden] = new HashSet<string>();
        _statusReasons[TileStatus.Highlighted] = new HashSet<string>();

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
        foreach (PlacementAction action in placement.RevealActions)
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

    public void RefreshStatusEffects()
    {
        var disabledReasons = _statusReasons[TileStatus.Disabled];
        var hiddenReasons = _statusReasons[TileStatus.Hidden];
        var highlightedReasons = _statusReasons[TileStatus.Highlighted];

        if (_originalMaterial == null)
        {
            _originalMaterial = transform.GetComponentsInChildren<SpriteRenderer>().Select(a => a.material).First();
        }

        float disabledScale = 0.5f;
        foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = disabledReasons.Any() ? new Color(disabledScale, disabledScale, disabledScale) : Color.white;
            sr.enabled = !hiddenReasons.Any();
            sr.material = highlightedReasons.Any() ? _highlightMaterial : (_originalMaterial != null ? _originalMaterial : sr.material);
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
