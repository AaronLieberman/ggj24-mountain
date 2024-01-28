using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkerPlan
{
    public Card Card;
    public Tile Tile;
}

public class Worker : MonoBehaviour
{
    public float Speed = 10f;

    public List<WorkerPlan> WorkerPlan { get; } = new();
    Vector2Int? _nextDestinationTileLoc;
    float _tileDistanceEpsilon = 0.01f;

    Transform _lastParent;
    TileGridLayout _map;
    Grid _grid;

    public void AddDestination(Card card, Tile tile)
    {
        WorkerPlan.Add(new WorkerPlan() { Card = card, Tile = tile });
        card.transform.parent = transform;
    }

    public bool IsHome
    {
        get
        {
            RefreshComponents();
            return !WorkerPlan.Any() && Utilities.ToVec2I(_grid.LocalToCell(transform.localPosition)) == _map.HomeLocation;
        }
    }

    void RefreshComponents()
    {
        if (_lastParent != transform.parent)
        {
            _lastParent = transform.parent;
            _map = GetComponentInParent<TileGridLayout>();
            _grid = GetComponentInParent<Grid>();
        }
    }

    Vector2Int GetNextDestinationWaypointCell()
    {
        return WorkerPlan.Any()
            ? Utilities.ToVec2I(_grid.LocalToCell(WorkerPlan.First().Tile.transform.localPosition))
            : _map.HomeLocation;
    }

    void Update()
    {
        RefreshComponents();

        if (WorkerPlan.Count == 0 && IsHome)
            return;

        if (_nextDestinationTileLoc == null)
        {
            var cell = _map.GetCellAtObject(transform);
            if (!_map.IsValidLocation(cell))
            {
                Debug.LogWarning($"Invalid cell location: {cell.x},{cell.y}");
                return;
            }

            var route = PathfinderAStar<Tile>.CalculateRoute(_map.GetTileFromLoc(cell), _map.GetTileFromLoc(GetNextDestinationWaypointCell()));
            _nextDestinationTileLoc = route.Any() ? route.First().Location : null;
            if (_nextDestinationTileLoc == null)
                return;
        }

        var nextDestinationTilePos = _grid.CellToLocal(new Vector3Int(_nextDestinationTileLoc.Value.x, _nextDestinationTileLoc.Value.y, 0));
        var differenceDir = new Vector3(nextDestinationTilePos.x, nextDestinationTilePos.y, 0) - transform.localPosition;
        if (differenceDir.magnitude < _tileDistanceEpsilon)
        {
            var cell = _map.GetCellAtObject(transform);

            // force the worker to be exactly in the right local position
            transform.localPosition = _grid.CellToLocal(Utilities.ToVec3I(_nextDestinationTileLoc.Value));

            if (_nextDestinationTileLoc == GetNextDestinationWaypointCell())
            {
                Debug.LogFormat("Reached waypoint {0} aka {1}", _nextDestinationTileLoc.Value, cell);
                WorkerPlan.RemoveAt(0);
            }

            _nextDestinationTileLoc = null;
        }
        else
        {
            var moveDir = differenceDir.normalized * Mathf.Min(Speed * Time.deltaTime, differenceDir.magnitude);
            transform.localPosition += moveDir;
        }
    }
}
