using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject WorkerPrefab;
    public GameObject ActorsContainer;

    public TileGridLayout Map => GetComponent<TileGridLayout>();

    public List<Worker> Workers = new List<Worker>();


    // Start is called before the first frame update
    void Start()
    {
        //var a = PathFinder.CalculateDistances( new Vector2Int(7, 7), 200);
        //var handUI = Utilities.GetRootComponentRecursive<HandUI>();
        // handUI.OnCardSelectStateChange += (card, enabled) =>
        // {
        //     if ( !enabled )
        //     {
        //         ClearCardTargets();
        //         return;
        //     }

        //     HighlightActionsForCard(card);
        // };
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Reset()
    {
        Map.Reset();
        ClearActors();
        AddWorkerAtHome();
    }

    //private List<Tile> _cardTargets = new List<Tile>();

    //public bool CanTarget(Tile tile) => _cardTargets.Contains(tile);

    public void ClearCardTargets()
    {
        // foreach (var t in _cardTargets)
        // {
        //     t.SetHighlight("cardTarget", false);
        // }

        // _cardTargets.Clear();
    }

    public void HighlightActionsForCard(Card card)
    {
        // ClearCardTargets();

        // foreach (var t in Map.EnumerateTiles()
        //                      .Where(t => (t.Placement == null
        //                                     || t.Placement.Actions.Count == 0
        //                                     || t.Placement.Actions.Any(a => a.Cost == card.name || string.IsNullOrEmpty(a.Cost)))
        //                                 && t.GetNeighbors().Any(n => !(n.Placement?.name ?? "Unexplored").StartsWith("Unexplored"))))
        // {
        //     t.SetHighlight("cardTarget", true);
        //     _cardTargets.Add(t);
        // }
    }


    public void ClearActors()
    {
        Utilities.DestroyAllChildren(ActorsContainer);
        Workers.Clear();
    }

    public void AddWorker(Vector2Int loc)
    {
        var workerObj = Instantiate(WorkerPrefab, ActorsContainer.transform);
        workerObj.transform.position = Map.GetPositionFromTileCoord(loc);
        var worker = workerObj.GetComponent<Worker>();

        Workers.Add(worker);
    }

    public void AddWorkerAtHome()
    {
        AddWorker(Map.HomeLocation);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Board))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var b = (Board)target;
        if (GUILayout.Button("Clear"))
        {
            b.ClearActors();
        }
    }
}
#endif