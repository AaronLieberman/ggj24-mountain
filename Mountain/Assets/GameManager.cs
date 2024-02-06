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

    public bool IsWorkRevealed { get; set; }
    public event EventHandler<string> ShowOnRevealedUI;

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
        SetWorkerAvailable(GetFirstAvailableWorker() != null);
    }

    Worker GetFirstAvailableWorker()
    {
        // update whether workers are available
        var workers = Map.GetComponentsInChildren<Worker>();
        return workers.FirstOrDefault(w => w.IsHome);
    }

    void SetWorkerAvailable(bool value)
    {
        if (value == IsWorkerAvailable) return;
        IsWorkerAvailable = value;
        WorkerAvailableChanged?.Invoke(null, null);
    }

    void StartGame()
    {
        IsWorkRevealed = false;

        SetupMap();

        Deck.Reset();
        Hand.Deck = Deck;
        Hand.Reset();

        Hand.DrawTillFull();

        WorkerPlanChanged?.Invoke(null, null);
    }

    void SetupMap()
    {
        Board.Reset();
    }

    public void OnMouseEnterTile(Tile tile)
    {
        StopMouseActiveHighlight();

        tile.SetHighlight("mouse", true);
        _highlightTile = tile;

        InvokeShowTooltip(tile.GetComponentInChildren<Placement>());
    }

    public void OnMouseExitTile(Tile tile)
    {
        tile.SetHighlight("mouse", false);
        StopMouseActiveHighlight();

        InvokeHideTooltip();
    }

    public void StopMouseActiveHighlight()
    {
        _highlightTile?.SetHighlight("mouse", false);
        _highlightTile = null;
    }

    public void AddCardToWorkerPlan(Card card, Tile tile)
    {
        if (!IsWorkerAvailable) return;
        var worker = GetFirstAvailableWorker();
        if (WorkerPlan.Count() >= MaxJourneySlots) return;

        var workerTile = Map.GetTileAtObject(worker.transform);

        var newDestination = new WorkerPlan() { Card = card, Tile = tile };
        if (!Map.IsPathPassable(workerTile, WorkerPlan.Select(a => a.Tile).Concat(new[] { newDestination.Tile }))) return;

        Debug.Log("Adding to worker plan:" + " | Selected Card: " + card.name + "  | Tile: " + tile.name);

        WorkerPlan.Add(newDestination);
        WorkerPlanChanged?.Invoke(null, null);

        Map.ShowPath(workerTile, WorkerPlan.Select(a => a.Tile));
    }

    public void ClearWorkerPlan()
    {
        WorkerPlan.Clear();
        Map.ClearPath();

        WorkerPlanChanged?.Invoke(null, null);
    }

    public void IncreaseJourneyPlanSlots()
    {
        MaxJourneySlots = MaxJourneySlots++;
        WorkerPlanChanged?.Invoke(null, null);
    }

    public void StartWorkerOnJourney()
    {
        if (!IsWorkerAvailable) return;

        var worker = GetFirstAvailableWorker();

        foreach (var plan in WorkerPlan)
        {
            worker.AddDestination(plan.Card, plan.Tile);
        }

        Hand.DrawTillFull();

        WorkerPlan.Clear();
        Map.ClearPath();

        WorkerPlanChanged?.Invoke(null, null);

        // Check for Game Over state
        if (Hand.GetHandCount() <= 0 && Deck.GetDeckCount() <= 0)
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

    Card _cardBeingConsidered;
    public void SetConsideringPlacingCard(Card card)
    {
        foreach (var tile in Map.GetComponentsInChildren<Tile>())
        {
            tile.SetDisabled("path", false);
        }

        if (_cardBeingConsidered == card) return;

        if (!IsWorkerAvailable) return;
        var worker = GetFirstAvailableWorker();
        if (WorkerPlan.Count() >= MaxJourneySlots) return;

        var workerTile = Map.GetTileAtObject(worker.transform);
        foreach (var tile in Map.GetComponentsInChildren<Tile>())
        {
            bool passable = Map.IsPathPassable(workerTile, WorkerPlan.Select(a => a.Tile).Concat(new[] { tile }));
            var placement = tile.GetComponentInChildren<Placement>();
            bool cardCanBePlaced = placement.Actions.Any(a => a.CanPayCost(card));

            tile.SetDisabled("path", !passable || !cardCanBePlaced);
        }
    }

    public bool CanCardBePlaced(Card card, Tile tile)
    {
        if (!IsWorkerAvailable) return false;
        var worker = GetFirstAvailableWorker();
        if (WorkerPlan.Count() >= MaxJourneySlots) return false;

        var workerTile = Map.GetTileAtObject(worker.transform);
        bool passable = Map.IsPathPassable(workerTile, WorkerPlan.Select(a => a.Tile).Concat(new[] { tile }));
        var placement = tile.GetComponentInChildren<Placement>();
        bool canCardBePlaced = placement.Actions.Any(a => a.CanPayCost(card));
        return canCardBePlaced;
    }

    public void ShowOnRevealText(string text)
    {
        ShowOnRevealedUI?.Invoke(null, text);
    }
}
