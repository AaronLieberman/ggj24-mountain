using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

using static UnityEngine.GraphicsBuffer;
using static UnityEditor.PlayerSettings;
using System.Drawing;
using UnityEngine.WSA;

[ExecuteInEditMode]
public class TileGridLayout : MonoBehaviour
{
    public Placement DefaultPrefab;
    public GameObject TilePrefab;
    public Placement HomePrefab;

    public Vector2Int HomeLocation;
    public Placement HomeInstance { get; private set;}

    public GameObject TilesContainer;
    public Vector2Int GridSize = new Vector2Int(20, 20);
    public Vector3 StartPos;

    public Bounds Bounds = new Bounds();

    private Tile[,] _tiles = new Tile[0, 0]; // col major

    private Vector2Int _generatedGridSize = new Vector2Int(-1, -1);

    void Start()
    {
    }

    public void Reset()
    {
        Generate(GridSize);
        HomeLocation = GetCenterTile();
        HomeInstance = CreateTileFromLoc(HomeLocation).SpawnPlacement(HomePrefab);
    }

    public void ClearTiles(bool clearCache)
    {
        Utilities.DestroyAllChildren(TilesContainer);

        Bounds = new Bounds();
        GetComponent<Tilemap>().ClearAllTiles();
        Array.Clear(_tiles, 0, _tiles.Length);

        if (clearCache)
        {
            _generatedGridSize = new Vector2Int(0, 0);
        }
    }

    public Vector2Int GetCenterTile()
        => new Vector2Int(GridSize.x / 2, GridSize.y / 2);

    public Vector3 GetPositionFromTileCoord(Vector2Int coord)
        => GetComponent<Grid>().GetCellCenterWorld(new Vector3Int(coord.x, coord.y, 0));



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
                CreateTileFromLoc(coord);
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

        foreach (var tile in GetComponentsInChildren<Tile>())
        {
            tile.SpawnPlacement(DefaultPrefab);
        }

    }

    public Tile GetTileFromLoc(Vector2Int coord)
        => _tiles[coord.x, coord.y];

    public Tile CreateTileFromLoc(Vector2Int coord)
    {
        var grid = GetComponent<Grid>();
        var pos = grid.GetCellCenterWorld(new Vector3Int(coord.x, coord.y, 0));
        var tileObj = Instantiate(TilePrefab, pos, Quaternion.identity, TilesContainer.transform);
        // not sure why this is necessary .. 
        tileObj.transform.localRotation = Quaternion.identity;
        tileObj.transform.Rotate(new Vector3(90, 0, 0));
        tileObj.name = $"Tile_{coord.x}_{coord.y}";
        var tile = tileObj.GetComponent<Tile>();
        tile.Location = coord;
        _tiles[coord.x, coord.y] = tile;
        return tile;
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
