using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //public TileGridLayout MapPrefab;

    public Worker WorkerPrefab;

    public TileGridLayout Map { get; private set; }
    public Deck Deck { get; private set; }
    public Hand Hand { get; private set; }

    public T GetRootComponent<T>() where T : Component
    {
        return SceneManager.GetActiveScene().GetRootGameObjects().Select(a => a.GetComponent<T>()).Single(a => a != null);
    }

    void Awake()
    {
        Map = GetRootComponent<TileGridLayout>();
        Map.name = $"Map";

        Deck = GetRootComponent<Deck>();
        Hand = GetRootComponent<Hand>();
    }

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        Deck.Reset();
        Hand.Deck = Deck;
        Hand.Reset();

        Hand.DrawTillFull();

        var worker = Instantiate(WorkerPrefab, Map.transform);

        worker.AddWaypoint(new Vector2Int(1, 5));
        worker.AddWaypoint(new Vector2Int(10, 6));
        worker.AddWaypoint(new Vector2Int(3, 2));
        worker.AddWaypoint(new Vector2Int(12, 14));

        // var b = new Vector3Int(1, 2, 0);
        // var o = Map.GetComponent<Grid>().CellToWorld(b);
        // Instantiate(WorkerPrefab, o, Quaternion.identity, Map.transform);
        // foreach (var coord in PathFinder.GetAdjacentHexCoords(new Vector2Int(b.x, b.y)))
        // {
        //     var world = Map.GetComponent<Grid>().CellToWorld(new Vector3Int(coord.x, coord.y, 0));
        //     var w = Instantiate(WorkerPrefab, world, Quaternion.identity, Map.transform);
        //     w.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        // }
    }

    void Update()
    {

    }
}
