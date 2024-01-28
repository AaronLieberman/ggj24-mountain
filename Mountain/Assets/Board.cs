using System.Collections;
using System.Collections.Generic;
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

        foreach (var coord in PathFinder.CalculateRoute(Map, new Vector2Int(1, 2), new Vector2Int(10, 6)))
        {
            var world = Map.GetComponent<Grid>().CellToWorld(new Vector3Int(coord.x, coord.y, 0));
            var w = Instantiate(WorkerPrefab, world, Quaternion.identity, Map.transform);
            w.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        }
    }

    public void ClearActors()
    {
        Utilities.DestroyAllChildren(ActorsContainer);
        Workers.Clear();
    }

    public void AddWorkerAtHome()
    {
        //Instantiate(HomePrefab, homeLoc, quaternion.identity, Map.transform);

        var workerObj = Instantiate(WorkerPrefab, ActorsContainer.transform);
        workerObj.transform.position = Map.GetPositionFromTileCoord(Map.HomeLocation);
        var worker = workerObj.GetComponent<Worker>();

        Workers.Add(worker);
    }
}


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