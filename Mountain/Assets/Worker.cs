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
    public float Speed = 0.1f;

    public List<WorkerPlan> WorkerPlan { get; } = new();
    Vector2Int? _nextDestinationTileLoc;
    float _tileDistanceEpsilon = 0.01f;

    public void AddDestination(Card card, Tile tile)
    {
        WorkerPlan.Add(new WorkerPlan() { Card = card, Tile = tile });
        card.transform.parent = transform;
    }

    void Update()
    {
        if (WorkerPlan.Count == 0)
            return;

        var map = GetComponentInParent<TileGridLayout>();
        var grid = GetComponentInParent<Grid>();

        if (_nextDestinationTileLoc == null)
        {
            var cell3D = grid.LocalToCell(new Vector3(transform.localPosition.x, transform.localPosition.y, 0));

            var destinationCell = grid.LocalToCell(WorkerPlan.First().Tile.transform.localPosition);
            var route = PathFinder.CalculateRoute(map, Utilities.ToVec2I(cell3D), Utilities.ToVec2I(destinationCell), 1);
            _nextDestinationTileLoc = route.SingleOrDefault();
            if (_nextDestinationTileLoc == null)
                return;
        }

        var nextDestinationTilePos = grid.CellToLocal(new Vector3Int(_nextDestinationTileLoc.Value.x, _nextDestinationTileLoc.Value.y, 0));
        var differenceDir = new Vector3(nextDestinationTilePos.x, nextDestinationTilePos.y, 0) - transform.localPosition;
        if (differenceDir.magnitude < _tileDistanceEpsilon)
        {
            var cell3D = grid.LocalToCell(new Vector3(transform.localPosition.x, transform.localPosition.y, 0));

            // force the worker to be exactly in the right local position
            transform.localPosition = grid.CellToLocal(Utilities.ToVec3I(_nextDestinationTileLoc.Value));

            var destinationCell = grid.LocalToCell(WorkerPlan.First().Tile.transform.localPosition);
            if (_nextDestinationTileLoc == Utilities.ToVec2I(destinationCell))
            {
                Debug.Log(string.Format("Reached waypoint {0} aka {1}", _nextDestinationTileLoc.Value, cell3D));
                WorkerPlan.RemoveAt(0);
            }

            _nextDestinationTileLoc = null;
        }
        else
        {
            var moveDir = differenceDir.normalized * Mathf.Min(Speed, differenceDir.magnitude);
            transform.localPosition += moveDir;
        }
    }
}
