using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkerPlan
{
    public Card Card;
    public Tile Tile;
}

public class GameManager : MonoBehaviour
{
    public int MaxJourneySlots = 2;

    public TileGridLayout Map { get; private set; }
    public Board Board { get; private set; }
    public Deck Deck { get; private set; }
    public Hand Hand { get; private set; }
    private GameOverUI GameOverUI { get; set; }

    public bool IsWorkerAvailable { get; private set; }
    public event EventHandler WorkerAvailableChanged;
    public event EventHandler WorkerPlanChanged;

    private Tile _highlightTile = null;
    public event EventHandler<Placement> ShowTooltip;
    public event EventHandler HideTooltip;

    public List<WorkerPlan> WorkerPlan { get; } = new();

    public bool IsDialogVisible { get; set; }
    public event EventHandler<string> ShowOnRevealedUI;

    public bool EnableDebugTools = false;

    void Awake()
    {
        Map = Utilities.GetRootComponentRecursive<TileGridLayout>();
        Board = Map.GetComponent<Board>();
        Map.name = $"Map";

        Deck = Utilities.GetRootComponent<Deck>();
        Hand = Utilities.GetRootComponent<Hand>();

        GameOverUI = Utilities.GetRootComponentRecursive<GameOverUI>();
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        var worker = GetFirstAvailableWorker();
        if (worker == IsWorkerAvailable) return;
        IsWorkerAvailable = worker;
        WorkerAvailableChanged?.Invoke(null, null);
    }

    Worker GetFirstAvailableWorker()
    {
        // update whether workers are available
        var workers = Map.GetComponentsInChildren<Worker>();
        return workers.FirstOrDefault(w => w.IsHome);
    }

    void StartGame()
    {
        IsDialogVisible = false;

        SetupMap();

        Deck.Reset();
        Hand.Deck = Deck;
        Hand.Reset();

        Hand.DrawTillFull();

        WorkerPlanChanged?.Invoke(null, null);

        OnNextTurn();
    }

    void SetupMap()
    {
        Board.Reset();
    }

    // This should drive the rest of the game instead of the other way around. One consequence of this way of doing it
    // is that multiple workers advance "turns" faster
    public void OnNextTurn()
    {
        RefreshPassability();
    }

    public void OnMouseEnterTile(Tile tile)
    {
        StopMouseActiveHighlight();

        tile.SetStatus(TileStatus.Highlighted, "mouse", true);
        _highlightTile = tile;

        InvokeShowTooltip(tile.Placement);
    }

    public void OnMouseExitTile(Tile tile)
    {
        tile.SetStatus(TileStatus.Highlighted, "mouse", false);
        StopMouseActiveHighlight();

        InvokeHideTooltip();
    }

    public void StopMouseActiveHighlight()
    {
        if (_highlightTile != null) _highlightTile.SetStatus(TileStatus.Highlighted, "mouse", false);
        _highlightTile = null;
    }

    public void AddCardToWorkerPlan(Card card, Tile tile)
    {
        var worker = GetFirstAvailableWorker();
        if (worker == null || WorkerPlan.Count() >= MaxJourneySlots) return;

        var workerTile = Map.GetTileAtObject(worker.transform);

        var newDestination = new WorkerPlan() { Card = card, Tile = tile };
        if (!Map.IsPathPassable(workerTile, WorkerPlan.Select(a => a.Tile).Concat(new[] { newDestination.Tile }))) return;

        Debug.Log("Adding to worker plan:" + " | Selected Card: " + card.name + "  | Tile: " + tile.name);

        WorkerPlan.Add(newDestination);
        WorkerPlanChanged?.Invoke(null, null);

        Map.ShowPath(workerTile, WorkerPlan.Select(a => a.Tile));

        SetConsideringPlacingCard(null);
    }

    public void ClearWorkerPlan()
    {
        WorkerPlan.Clear();
        Map.ClearPath();

        WorkerPlanChanged?.Invoke(null, null);
    }

    public void IncreaseJourneyPlanSlots()
    {
        MaxJourneySlots = ++MaxJourneySlots;
        WorkerPlanChanged?.Invoke(null, null);
    }

    public void StartWorkerOnJourney()
    {
        var worker = GetFirstAvailableWorker();
        if (worker == null) return;

        foreach (var plan in WorkerPlan)
        {
            worker.AddDestination(plan.Card, plan.Tile);
        }

        Hand.DrawTillFull();

        WorkerPlan.Clear();
        Map.ClearPath();

        WorkerPlanChanged?.Invoke(null, null);

        // Check for Game Over state
        if (Hand.GetHandCount() <= 0 && Deck.GetDeckCount() <= 0 && GameOverUI != null)
        {
            GameOverUI.ShowGameOverUI();
        }
    }

    public void InvokeShowTooltip(Placement placement)
    {
        if (placement != null)
        {
            ShowTooltip?.Invoke(null, placement);
        }
    }

    public void InvokeHideTooltip()
    {
        HideTooltip?.Invoke(null, null);
    }

    public void RefreshPassability()
    {
        var unexploredDistances = PathFinder.CalculateUnexploredDistance(Utilities.GetRootComponent<TileGridLayout>().HomeLocation, MaxJourneySlots);

        foreach (var tile in Map.GetComponentsInChildren<Tile>())
        {
            var visible = tile.IsRevealed || (unexploredDistances.ContainsKey(tile.Location) &&
                (!unexploredDistances[tile.Location].Passable ||
                    unexploredDistances[tile.Location].UnexploredDistance <= MaxJourneySlots));
            tile.SetStatus(TileStatus.Hidden, "passability", !visible);

            bool staticlyImpassable = !tile.Placement.IsPassable && !tile.Placement.Actions.Any();
            tile.SetStatus(TileStatus.StaticallyImpassbile, "staticallyImpassible", staticlyImpassable);
        }

        SetConsideringPlacingCard(null);
    }

    public void SetConsideringPlacingCard(Card card)
    {
        // initially, enable all tiles if they're passable
        foreach (var tile in Map.GetComponentsInChildren<Tile>())
        {
            tile.SetStatus(TileStatus.Disabled, "place card", false);
        }

        if (card == null) return;

        var worker = GetFirstAvailableWorker();
        if (worker == null) return;

        var startingTile = WorkerPlan.Count == 0
            ? Map.GetTileAtObject(worker.transform)
            : WorkerPlan.Last().Tile;
        foreach (var tile in Map.GetComponentsInChildren<Tile>())
        {
            if (!tile.GetStatus(TileStatus.Hidden))
            {
                bool cardCanBePlaced =
                    tile.Placement.Actions.Any(a => a.CanPayCost(card)) &&
                    !WorkerPlan.Any(p => p.Tile == tile);

                if (cardCanBePlaced)
                {
                    // if the card could be placed in this tile, make sure that there's a path in which to do so
                    var route = TilePathfinderAStar.CalculateRoute(startingTile, tile);
                    if (route != null)
                    {
                        bool passable = true;
                        // skip the first (starting location) and last items (one we're placing the card into) in the route
                        foreach (var stepTile in route.Skip(1).Take(route.Count() - 2))
                        {
                            if (!stepTile.Placement.IsPassable)
                            {
                                passable = false;
                                break;
                            }
                        }

                        if (passable)
                        {
                            // we're already set to disabled=false from the top of this function so we're good to go
                            continue;
                        }
                    }
                }
            }

            tile.SetStatus(TileStatus.Disabled, "place card", true);
        }
    }

    public bool CanCardBePlaced(Card card, Tile tile)
    {
        var worker = GetFirstAvailableWorker();
        if (worker == null || WorkerPlan.Count() >= MaxJourneySlots) return false;

        var workerTile = Map.GetTileAtObject(worker.transform);
        bool passable = Map.IsPathPassable(workerTile, WorkerPlan.Select(a => a.Tile).Concat(new[] { tile }));
        bool canCardBePlaced = tile.Placement.Actions.Any(a => a.CanPayCost(card));
        return canCardBePlaced;
    }

    public void ShowOnRevealText(string text)
    {
        ShowOnRevealedUI?.Invoke(null, text);
    }
}
