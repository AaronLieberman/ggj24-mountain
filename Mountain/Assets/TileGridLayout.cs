using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;
using static UnityEditor.PlayerSettings;
using System.Drawing;
using UnityEngine.WSA;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class TileGridLayout : MonoBehaviour
{
    public Placement DefaultPrefab;
    public GameObject TilePrefab;
    public Placement HomePrefab;
    public Material HighlightMaterial;

    public Vector2Int HomeLocation;
    public Placement HomeInstance { get; private set; }

    public GameObject TilesContainer;
    public Vector2Int GridSize = new Vector2Int(20, 20);

    public Bounds Bounds = new Bounds();

    private Tile[,] _tiles = new Tile[0, 0]; // col major

    private Vector2Int _generatedGridSize = new Vector2Int(-1, -1);


    public LineRenderer PathLines => GetComponentInChildren<LineRenderer>();

    void Start()
    {
    }

    public void Reset()
    {
        Generate(GridSize);
        HomeLocation = GetCenterTile();
        HomeInstance = GetTileFromLoc(HomeLocation).SpawnPlacement(HomePrefab);
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

    public Tile GetTileAtObject(Transform transform)
    {
        var cell = GetCellAtObject(transform);
        return GetTileFromLoc(cell);
    }

    public Vector2Int GetCellAtObject(Transform transform)
    {
        return Utilities.ToVec2I(GetComponent<Grid>().LocalToCell(new Vector3(transform.localPosition.x, transform.localPosition.y, 0)));
    }

    public void ClearPath()
    {
        foreach (var tile in GetComponentsInChildren<Tile>())
        {
            tile.SetHighlight("path", false);
        }
    }

    public void ShowPath(Tile startTile, IEnumerable<Tile> destinations)
    {
        ClearPath();

        var currentTile = startTile;
        foreach (var destination in destinations)
        {
            foreach (var tile in PathfinderAStar<Tile>.CalculateRoute(currentTile, destination))
            {
                tile.SetHighlight("path", true);
            }

            currentTile = destination;
        }
    }

    public void OnMouseEnterTile(Tile tile)
    {
    }

    public void OnMouseDownTile(Tile tile)
    {
    }

    public void OnMouseUpTile(Tile tile)
    {
        var handUI = Utilities.GetRootComponents<Canvas>()
			.Select(c => c.GetComponentInChildren<HandUI>())
			.First();

        CardUI selectedCardUI = handUI.SelectedCardUI;
        if (selectedCardUI != null)
        {
            Card selectedCard = selectedCardUI.Card;
            Utilities.GetRootComponent<GameManager>().AddCardToWorkerPlan(selectedCard, tile);
        }
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
            -Bounds.size.x / 2,
            0,
            0);

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

    public bool IsValidLocation(Vector2Int checkCoord)
        => checkCoord.x >= 0
            && checkCoord.x < GridSize.x
            && checkCoord.y >= 0
            && checkCoord.y < GridSize.y;

    void Update()
    {
#if UNITY_EDITOR
        if (!_generatedGridSize.Equals(GridSize))
        {
            Generate(GridSize);
        }
#endif

        //if (_pathfindingPath?.Length ?? 0) > 1)
        //{
        //    PathLines.positionCount = _pathfindingPath.Length; 
        //    for ( var i = 0; i < _pathfindingPath.Length; ++i )
        //    {
        //        PathLines.SetPosition(i, GetPositionFromTileCoord(_pathfindingPath[i]));
        //    }
        //}
    }

    public IEnumerable<Tile> GetNeighborsByTile(Tile tile)
        => PathFinder.GetAdjacentHexCoords(new Vector2Int(tile.Location.x, tile.Location.y))
            .Where(checkCoord => IsValidLocation(checkCoord))
            .Select(coord => GetTileFromLoc(coord));

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
