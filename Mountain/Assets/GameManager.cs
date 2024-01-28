using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlacementMode
{
    None,
    PlanWorker,
}

public class GameManager : MonoBehaviour
{
    public TileGridLayout Map { get; private set; }
    public Board Board { get; private set; }
    public Deck Deck { get; private set; }
    public Hand Hand { get; private set; }

    public PlacementMode PlacementMode { get; private set; }
    public event EventHandler PlacementModeChanged;

    public bool IsWorkerAvailable { get; private set; }
    public event EventHandler WorkerAvailableChanged;

    List<WorkerPlan> _workerPlan = new();

    void Awake()
    {
        Board = Utilities.GetRootComponent<Board>();
        Map = Utilities.GetRootComponent<TileGridLayout>();
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
        var grid = Map.GetComponent<Grid>();
        var homeCell = grid.LocalToCell(Map.HomeInstance.transform.localPosition);

        // update whether workers are available
        var workers = Map.GetComponentsInChildren<Worker>();
        return workers.FirstOrDefault(w => grid.LocalToCell(w.transform.localPosition) == homeCell);
    }

    void SetWorkerAvailable(bool value)
    {
        if (value == IsWorkerAvailable) return;
        IsWorkerAvailable = value;
        WorkerAvailableChanged.Invoke(null, null);
    }

    void SetPlacementMode(PlacementMode value)
    {
        if (value == PlacementMode) return;
        PlacementMode = value;
        PlacementModeChanged.Invoke(null, null);
    }

    void StartGame()
    {
        SetupMap();

        Deck.Reset();
        Hand.Deck = Deck;
        Hand.Reset();

        Hand.DrawTillFull();
    }

    void SetupMap()
    {
        Board.Reset();
        //var homeLoc = Map.GetComponent<Grid>().CellToWorld(new Vector3Int(10, 6, 0));
        //Instantiate(HomePrefab, homeLoc, quaternion.identity, Map.transform);

        // var worker = Instantiate(WorkerPrefab, Map.transform);

        // worker.AddWaypoint(new Vector2Int(1, 5));
        // worker.AddWaypoint(new Vector2Int(10, 6));
        // worker.AddWaypoint(new Vector2Int(3, 2));
        // worker.AddWaypoint(new Vector2Int(12, 14));

        // var b = new Vector3Int(1, 2, 0);
        // var o = Map.GetComponent<Grid>().CellToWorld(b);
        // Instantiate(WorkerPrefab, o, Quaternion.identity, Map.transform);
        // foreach (var coord in PathFinder.GetAdjacentHexCoords(new Vector2Int(b.x, b.y)))
        // {
        //     var world = Map.GetComponent<Grid>().CellToWorld(new Vector3Int(coord.x, coord.y, 0));
        //     var w = Instantiate(WorkerPrefab, world, Quaternion.identity, Map.transform);
        //     w.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        // }

        //foreach (var coord in PathFinder.CalculateRoute(Map, new Vector2Int(1, 2), new Vector2Int(10, 6)))
        //{
        //    var world = Map.GetComponent<Grid>().CellToWorld(new Vector3Int(coord.x, coord.y, 0));
        //    var w = Instantiate(WorkerPrefab, world, Quaternion.identity, Map.transform);
        //    w.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        //}
    }

    public void StartPlacingWorker()
    {
        SetPlacementMode(PlacementMode.PlanWorker);
    }

    public void AddCardToWorkerPlan(Card card, Tile tile)
    {
        if (!IsWorkerAvailable) return;
        var worker = GetFirstAvailableWorker();
        if (worker.GetComponentsInChildren<Card>().Count() > worker.MaxCards) return;
        _workerPlan.Add(new WorkerPlan() { Card = card, Tile = tile });
    }

    public void StartWorkerOnJourney()
    {
        if (PlacementMode != PlacementMode.PlanWorker) return;
        if (!IsWorkerAvailable) return;

        var worker = GetFirstAvailableWorker();

        foreach (var plan in _workerPlan)
        {
            plan.Card.transform.parent = worker.transform;
        }
    }
}
