using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

using static UnityEngine.GraphicsBuffer;
using static UnityEditor.PlayerSettings;

[ExecuteInEditMode]
public class TileGridLayout : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject TilesContainer;
    public Vector2Int GridSize = new Vector2Int(20, 20);
    public Vector3 StartPos;

    public Bounds Bounds = new Bounds();

    private Tile[,] _tiles = new Tile[0, 0]; // col major

    private Vector2Int _generatedGridSize = new Vector2Int(-1, -1);

    void Start()
    {
        Generate(GridSize);
    }

    public void ClearTiles(bool clearCache)
    {
        for (int i = TilesContainer.transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(TilesContainer.transform.GetChild(i).gameObject);
        }

        Bounds = new Bounds();
        GetComponent<Tilemap>().ClearAllTiles();
        Array.Clear(_tiles, 0, _tiles.Length);

        if (clearCache)
        {
            _generatedGridSize = new Vector2Int(0, 0);
        }
    }

    public void ClearPath()
    {

    }
    public void ShowPath(Tile src, Tile dest)
    {

    }

    public void OnMouseEnterTile(Tile tile)
    {
        if (_clicked != null)
        {
            ShowPath(_clicked, tile);
        }
    }

    private Tile _clicked;
    public void OnMouseDownTile(Tile tile)
    {
        _clicked = tile;
    }

    public void Generate(Vector2Int size)
    {
        if (TilePrefab == null
            || TilesContainer == null)
        {
            return;
        }

        ClearTiles(true);

        var grid = GetComponent<Grid>();

        _tiles = new Tile[size.x, size.y];
        for (int x = 0; x < size.x; ++x)
        {
            for (int y = 0; y < size.y; ++y)
            {
                var coord = new Vector2Int(x, y);
                Tile tile = GetTileFromLoc(coord);
                tile.name = $"Tile_{x}_{y}";
                tile.Location = coord;
                _tiles[x, y] = tile;
            }
        }

        Action refreshBounds = () =>
        {
            if (_tiles.Length > 0)
            {
                Bounds = new Bounds(_tiles[0, 0].transform.position, grid.cellSize);
                Bounds.Encapsulate(new Bounds(_tiles[size.x - 1, size.y - 1].transform.position, grid.cellSize));
            }
        };

        refreshBounds();

        // anchor along cameraX
        transform.position = new Vector3(
            StartPos.x - Bounds.size.x / 2,
            StartPos.y,
            StartPos.z);

        refreshBounds();

        _generatedGridSize = size;
    }

    public Tile GetTileFromLoc(Vector2Int coord)
    {
        var grid = GetComponent<Grid>();
        var pos = grid.GetCellCenterWorld(new Vector3Int(coord.x, coord.y, 0));
        var tileObj = Instantiate(TilePrefab, pos, Quaternion.identity, TilesContainer.transform);
        // not sure why this is necessary .. 
        tileObj.transform.localRotation = Quaternion.identity;
        return tileObj.GetComponent<Tile>();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!_generatedGridSize.Equals(GridSize))
        {
            Generate(GridSize);
        }
#endif
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
            gl.ClearTiles(false);
        }
        else if (GUILayout.Button("Regenerate"))
        {
            gl.Generate(gl.GridSize);
        }
    }


}
