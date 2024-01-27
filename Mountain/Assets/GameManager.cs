using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Worker WorkerPrefab;
    public Placement HomePrefab;

    public Placement DefaultPrefab;

    public TileGridLayout Map { get; private set; }
    public Deck Deck { get; private set; }
    public Hand Hand { get; private set; }



    void Awake()
    {
        Map = Utilities.GetRootComponent<TileGridLayout>();
        Map.name = $"Map";

        Deck = Utilities.GetRootComponent<Deck>();
        Hand = Utilities.GetRootComponent<Hand>();
    }

    void Start()
    {
        StartGame();
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
        var homeLoc = new Vector2Int(10, 6);
        Map.GetTileFromLoc(homeLoc).SpawnPlacement(HomePrefab);
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

        foreach (var tile in Map.GetComponentsInChildren<Tile>())
        {
            tile.SpawnPlacement(DefaultPrefab);
        }

        foreach (var coord in PathFinder.CalculateRoute(Map, new Vector2Int(1, 2), new Vector2Int(10, 6)))
        {
            var world = Map.GetComponent<Grid>().CellToWorld(new Vector3Int(coord.x, coord.y, 0));
            var w = Instantiate(WorkerPrefab, world, Quaternion.identity, Map.transform);
            w.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        }
    }

    public bool IsWorkerAvailable()
    {
        // var homePlacement = Map.GetComponentInChildren<
        return false;
    }
}
