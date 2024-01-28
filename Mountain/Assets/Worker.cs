using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public float Speed = 10f;

    public List<(Card, Tile)> WorkerPlan { get; } = new();
    Vector2Int? _nextDestinationTileLoc;
    float _tileDistanceEpsilon = 0.01f;

    Transform _lastParent;
    TileGridLayout _map;
    Grid _grid;

    public void AddDestination(Card card, Tile tile)
    {
        WorkerPlan.Add((card, tile));
        card.transform.parent = transform;
    }

    public bool IsHome
    {
        get
        {
            RefreshComponents();
            return !WorkerPlan.Any() && (transform.localPosition - _map.HomeInstance.transform.parent.localPosition).magnitude < _tileDistanceEpsilon;
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
            ? Utilities.ToVec2I(_grid.LocalToCell(WorkerPlan.First().Item2.transform.localPosition))
            : _map.HomeLocation;
    }

    void Update()
    {
        RefreshComponents();

        if (IsHome) return;

        if (_nextDestinationTileLoc == null)
        {
            var cell = _map.GetCellAtObject(transform);
            if (!_map.IsValidLocation(cell))
            {
                Debug.LogWarning($"Invalid cell location: {cell.x},{cell.y}");
                return;
            }

            var route = PathfinderAStar<Tile>.CalculateRoute(_map.GetTileFromLoc(cell), _map.GetTileFromLoc(GetNextDestinationWaypointCell()));
            _nextDestinationTileLoc = route != null && route.Count > 1 ? route.Skip(1).First().Location : null;
            if (_nextDestinationTileLoc == null)
                return;
        }

        var nextDestinationTilePos = _grid.CellToLocal(Utilities.ToVec3I(_nextDestinationTileLoc.Value));
        var differenceDir = new Vector3(nextDestinationTilePos.x, nextDestinationTilePos.y, 0) - transform.localPosition;
        if (differenceDir.magnitude < _tileDistanceEpsilon)
        {
            var cell = _map.GetCellAtObject(transform);

            // force the worker to be exactly in the right local position
            transform.localPosition = _grid.CellToLocal(Utilities.ToVec3I(_nextDestinationTileLoc.Value));

            if (_nextDestinationTileLoc == GetNextDestinationWaypointCell())
            {
                if (IsHome)
                {
                    Debug.LogFormat("Reached home {0} aka {1}", _nextDestinationTileLoc.Value, cell);
                }
                else
                {
                    Debug.LogFormat("Reached waypoint {0} aka {1}", _nextDestinationTileLoc.Value, cell);
                    var currentPlan = WorkerPlan.First();
                    WorkerPlan.RemoveAt(0);
                    ExecutePlan(currentPlan.Item1, currentPlan.Item2);
                }
            }

            _nextDestinationTileLoc = null;
        }
        else
        {
            var moveDir = differenceDir.normalized * Mathf.Min(Speed * Time.deltaTime, differenceDir.magnitude);
            transform.localPosition += moveDir;
        }
    }

    void ExecutePlan(Card card, Tile tile)
    {
        card.SpawnOnTile(tile);
    }
}
