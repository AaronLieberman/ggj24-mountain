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

    public void Reset()
    {
        Map.Reset();
        ClearActors();
        AddWorkerAtHome();
    }

    public void ClearActors()
    {
        Utilities.DestroyAllChildren(ActorsContainer.transform);
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