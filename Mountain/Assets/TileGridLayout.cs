using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Drawing;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

[ExecuteInEditMode]
public class TileGridLayout : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject TilesContainer;
    public Vector2Int GridSize = new Vector2Int(20, 20);
    public Vector2Int StartPos;

    public Bounds Bounds = new Bounds();

    private Tile[,] _tiles = new Tile[0,0]; // col major

    private Vector2Int _generatedGridSize = new Vector2Int(-1, -1);

    void Start()
    {
        Generate(GridSize);
    }

    public void ClearTiles()
    {
        for ( int i = TilesContainer.transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(TilesContainer.transform.GetChild(i).gameObject);
        }

        Bounds = new Bounds();
        GetComponent<Tilemap>().ClearAllTiles();
        Array.Clear(_tiles, 0, _tiles.Length);

        _generatedGridSize = new Vector2Int(0, 0);
    }

    public void Generate(Vector2Int size)
    {
        if (TilePrefab == null
            || TilesContainer == null)
        {
            return;
        }

        ClearTiles();

        var grid = GetComponent<Grid>();

        _tiles = new Tile[size.x, size.y];
        for (int x = 0; x < size.x; ++x)
        {
            for (int y = 0; y < size.y; ++y)
            {
                var coord = new Vector2Int(x, y);
                var pos = grid.GetCellCenterWorld(new Vector3Int(coord.x, coord.y, 0));
                var tileObj = Instantiate(TilePrefab, pos, Quaternion.identity, TilesContainer.transform);
                var tile = tileObj.GetComponent<Tile>();
                tile.name = $"Tile_{x}_{y}";
                tile.Location = coord;
                _tiles[x, y] = tile;
                Bounds.Encapsulate(pos);
            }
        }

        _generatedGridSize = size;
    }

    void Update()
    {
        if (!_generatedGridSize.Equals(GridSize))
        {
            Generate(GridSize);
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;

        //Gizmos.DrawLine(transform.position, transform.position + new Vector3(100, 100, 100));

        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }
}

[CustomEditor(typeof(TileGridLayout))]
public class TileGridLayoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var gl = (TileGridLayout)target;
        if (GUILayout.Button("Clear"))
        {
            gl.GridSize.x = 0;
            gl.GridSize.y = 0;
            gl.Generate(gl.GridSize);
        }
        else if (GUILayout.Button("Regenerate"))
        {
            gl.Generate(gl.GridSize);
        }
    }


}
