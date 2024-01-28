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
    public int MaxCards = 2;

    public TileGridLayout Map { get; private set; }
    public Board Board { get; private set; }
    public Deck Deck { get; private set; }
    public Hand Hand { get; private set; }

    public bool IsWorkerAvailable { get; private set; }
    public event EventHandler WorkerAvailableChanged;
    public event EventHandler WorkerPlanChanged;

    private Tile _highlightTile = null;
    public event EventHandler ShowTooltip;
    public event EventHandler HideTooltip;

    public List<WorkerPlan> WorkerPlan { get; } = new();

    void Awake()
    {
        Map = Utilities.GetRootComponentRecursive<TileGridLayout>();
        Board = Map.GetComponent<Board>();
        Map.name = $"Map";

        Deck = Utilities.GetRootComponent<Deck>();
        Hand = Utilities.GetRootComponent<Hand>();
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
        WorkerAvailableChanged.Invoke(null, null);
    }

    void StartGame()
    {
        SetupMap();

        Deck.Reset();
        Hand.Deck = Deck;
        Hand.Reset();

        Hand.DrawTillFull();

        WorkerPlanChanged.Invoke(null, null);
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

        InvokeShowTooltip();
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

    public void OnMouseDownTile(Tile tile)
    {
        //AddCardToWorkerPlan(Hand.GetComponentInChildren<Card>(), tile);
    }
    
    public void OnMouseOverTile(Tile tile)
    {
        /*
        if (Input.GetMouseButtonDown(1))
        {
            StartWorkerOnJourney();
        }*/
    }

    public void AddCardToWorkerPlan(Card card, Tile tile)
    {
        if (!IsWorkerAvailable) return;
        var worker = GetFirstAvailableWorker();
        if (WorkerPlan.Count() >= MaxCards) return;

        Debug.Log("Adding to worker plan:" + " | Selected Card: " + card.name + "  | Tile: " + tile.name);

        WorkerPlan.Add(new WorkerPlan() { Card = card, Tile = tile });
        WorkerPlanChanged.Invoke(null, null);

        var workerTile = Map.GetTileAtObject(worker.transform);
        Map.ShowPath(workerTile, WorkerPlan.Select(a => a.Tile));
    }

    public void ClearWorkerPlan()
    {
        WorkerPlan.Clear();
        Map.ClearPath();

        WorkerPlanChanged.Invoke(null, null);
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

        WorkerPlanChanged.Invoke(null, null);
    }

    public void InvokeShowTooltip()
    {
        ShowTooltip.Invoke(null, null);
    }

    public void InvokeHideTooltip()
    {
        HideTooltip.Invoke(null, null);
    }
}
