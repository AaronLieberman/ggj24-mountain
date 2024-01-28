using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int MaxCards = 2;

    public TileGridLayout Map { get; private set; }
    public Board Board { get; private set; }
    public Deck Deck { get; private set; }
    public Hand Hand { get; private set; }

    public bool IsWorkerAvailable { get; private set; }
    public event EventHandler WorkerAvailableChanged;

    public List<WorkerPlan> WorkerPlan { get; } = new();

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
    }

    void SetupMap()
    {
        Board.Reset();

        #region Debug code for hexes
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
        #endregion
    }

    public void OnMouseEnterTile(Tile tile)
    {
    }

    public void OnMouseDownTile(Tile tile)
    {
        AddCardToWorkerPlan(Hand.GetComponentInChildren<Card>(), tile);
    }
    
    public void OnMouseOverTile(Tile tile)
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartWorkerOnJourney();
        }
    }

    public void AddCardToWorkerPlan(Card card, Tile tile)
    {
        if (!IsWorkerAvailable) return;
        var worker = GetFirstAvailableWorker();
        if (worker.GetComponentsInChildren<Card>().Count() > MaxCards) return;
        WorkerPlan.Add(new WorkerPlan() { Card = card, Tile = tile });

        var workerTile = Map.GetTileAtObject(worker.transform);
        Map.ShowPath(workerTile, WorkerPlan.Select(a => a.Tile));
    }

    public void ClearWorkerPlan()
    {
        WorkerPlan.Clear();
        Map.ClearPath();
    }

    public void StartWorkerOnJourney()
    {
        if (!IsWorkerAvailable) return;

        var worker = GetFirstAvailableWorker();

        foreach (var plan in WorkerPlan)
        {
            worker.AddDestination(plan.Card, plan.Tile);
        }

        Map.ClearPath();
    }
}
