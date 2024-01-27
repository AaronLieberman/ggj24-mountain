using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //public TileGridLayout MapPrefab;

    public TileGridLayout Map { get; private set; }
    public Deck Deck { get; private set; }
    public Hand Hand { get; private set; }

    public T GetRootComponent<T>()
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
    }
}
