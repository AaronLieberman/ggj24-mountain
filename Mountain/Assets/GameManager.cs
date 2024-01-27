using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GameManager : MonoBehaviour
{
    public TileGridLayout MapPrefab;

    public TileGridLayout Map { get; private set; }

    void Start()
    {
        var mapObj = Instantiate(MapPrefab, transform.position, Quaternion.identity, transform.parent);
        mapObj.name = $"Map";
        Map = mapObj.GetComponent<TileGridLayout>();
    }

    void Update()
    {
    }
}
