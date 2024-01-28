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
            var homeCell = _grid.LocalToCell(_map.HomeInstance.transform.parent.localPosition);
            return WorkerPlan.Any() && _grid.LocalToCell(transform.localPosition) == homeCell;
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
            var cell3D = Utilities.ToVec2I(_grid.LocalToCell(new Vector3(transform.localPosition.x, transform.localPosition.y, 0)));

            var route = PathFinder.CalculateRoute(_map, cell3D, GetNextDestinationWaypointCell(), 1);
            _nextDestinationTileLoc = route.SingleOrDefault();
            if (_nextDestinationTileLoc == null)
                return;
        }

        var nextDestinationTilePos = _grid.CellToLocal(new Vector3Int(_nextDestinationTileLoc.Value.x, _nextDestinationTileLoc.Value.y, 0));
        var differenceDir = new Vector3(nextDestinationTilePos.x, nextDestinationTilePos.y, 0) - transform.localPosition;
        if (differenceDir.magnitude < _tileDistanceEpsilon)
        {
            var cell3D = _grid.LocalToCell(new Vector3(transform.localPosition.x, transform.localPosition.y, 0));

            // force the worker to be exactly in the right local position
            transform.localPosition = _grid.CellToLocal(Utilities.ToVec3I(_nextDestinationTileLoc.Value));

            if (_nextDestinationTileLoc == GetNextDestinationWaypointCell())
            {
                Debug.Log(string.Format("Reached waypoint {0} aka {1}", _nextDestinationTileLoc.Value, cell3D));
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
