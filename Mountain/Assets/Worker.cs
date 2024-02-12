using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public float Speed = 10f;

    public IReadOnlyList<WorkerPlan> WorkerPlans { get => _workerPlans; }
    Vector2Int? _nextDestinationTileLoc;
    float _tileDistanceEpsilon = 0.01f;

    Transform _lastParent;
    TileGridLayout _map;
    Grid _grid;
    readonly List<WorkerPlan> _workerPlans = new();

    public void AddDestination(Card card, Tile tile)
    {
        _workerPlans.Add(new WorkerPlan() { Card = card, Tile = tile });
        card.transform.parent = transform;
    }

    WorkerPlan GetFirstValidWorkerPlan()
    {
        return _workerPlans.FirstOrDefault(p => p.Tile.Placement.Actions.Any(a => a.CanPayCost(p.Card)));
    }

    public void ClearWorkerPlans()
    {
        while (WorkerPlans.Count > 0)
        {
            Utilities.GetRootComponent<Deck>().MoveCardToDeck(WorkerPlans.Last().Card);
            _workerPlans.RemoveAt(_workerPlans.Count - 1);
        }
    }

    public bool IsHome
    {
        get
        {
            RefreshComponents();
            return GetFirstValidWorkerPlan() == null && (transform.localPosition - _map.HomeInstance.transform.parent.localPosition).magnitude < _tileDistanceEpsilon;
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
        var firstValidWorkerPlan = GetFirstValidWorkerPlan();
        return firstValidWorkerPlan != null
            ? Utilities.ToVec2I(_grid.LocalToCell(firstValidWorkerPlan.Tile.transform.localPosition))
            : _map.HomeLocation;
    }

    void Update()
    {
        RefreshComponents();

        if (_nextDestinationTileLoc == null && IsHome) return;

        if (_nextDestinationTileLoc == null)
        {
            var cell = _map.GetCellAtObject(transform);
            if (!_map.IsValidLocation(cell))
            {
                Debug.LogWarning($"Invalid cell location: {cell.x},{cell.y}");
                return;
            }

            List<Tile> route;
            while (_workerPlans.Any())
            {
                var plan = _workerPlans.First();

                // CalculateRoute cheats a bit in that due to how GetHeuristic is implemented, the last item on the
                // route is always considered passible, even when it's not. That allows us to decide on whether
                // you're allowed to enter the tile here
                route = TilePathfinderAStar.CalculateRoute(_map.GetTileFromLoc(cell), plan.Tile);
                if (route != null && route.Count > 1)
                {
                    if (route.ElementAt(1).Placement.PathingHeuristic < 10000 ||
                    plan.Tile.Placement.Actions.Any(action => action.CanPayCost(plan.Card)))
                    {
                        _nextDestinationTileLoc = route.ElementAt(1).Location;
                        break;
                    }
                }

                _workerPlans.RemoveAt(0);
                if (plan.Card != null)
                {
                    Utilities.GetRootComponent<Deck>().MoveCardToDeck(plan.Card);
                }
            }

            if (_nextDestinationTileLoc == null)
            {
                route = TilePathfinderAStar.CalculateRoute(_map.GetTileFromLoc(cell), _map.GetTileFromLoc(_map.HomeLocation));
                _nextDestinationTileLoc = route != null && route.Count > 1 ? route.ElementAt(1).Location : null;
            }
        }

        var nextDestinationTilePos = _grid.CellToLocal(Utilities.ToVec3I(_nextDestinationTileLoc.Value));
        var differenceDir = new Vector3(nextDestinationTilePos.x, nextDestinationTilePos.y, 0) - transform.localPosition;
        if (differenceDir.magnitude < _tileDistanceEpsilon)
        {
            var cell = _map.GetCellAtObject(transform);

            // force the worker to be exactly in the right local position
            transform.localPosition = _grid.CellToLocal(Utilities.ToVec3I(_nextDestinationTileLoc.Value));

            bool executeOnVisit = true;

            if (_nextDestinationTileLoc == GetNextDestinationWaypointCell())
            {
                if (IsHome)
                {
                    Debug.LogFormat("Reached home {0} aka {1}", _nextDestinationTileLoc.Value, cell);

                    //put any leftover cards on the explorer back to the top of the deck
                    Card[] leftoverCards = GetComponentsInChildren<Card>();
                    foreach (Card c in leftoverCards)
                    {
                        Utilities.GetRootComponent<Deck>().MoveCardToDeck(c);
                    }

                    gameObject.transform.SetParent(null);
                    GameObject.Destroy(gameObject);
                    Utilities.GetRootComponent<Board>().AddWorkerAtHome();
                    Utilities.GetRootComponent<Hand>().DrawTillFull();
                }
                else
                {
                    Debug.LogFormat("Reached waypoint {0} aka {1}", _nextDestinationTileLoc.Value, cell);
                    var currentPlan = GetFirstValidWorkerPlan();
                    if (currentPlan != null)
                    {
                        _workerPlans.Remove(currentPlan);
                        if (currentPlan.Card != null)
                        {
                            ExecutePlan(currentPlan.Card, currentPlan.Tile, out executeOnVisit);
                        }
                    }
                }
            }

            if (executeOnVisit)
            {
                OnVisit(_map.GetTileFromLoc(cell));
            }

            _nextDestinationTileLoc = null;

            Utilities.GetRootComponent<GameManager>().OnNextTurn();
        }
        else
        {
            var moveDir = differenceDir.normalized * Mathf.Min((Utilities.GetRootComponent<GameManager>().IsWorkRevealed ? 0 : Speed) * Time.deltaTime, differenceDir.magnitude);
            transform.localPosition += moveDir;
        }
    }

    void ExecutePlan(Card card, Tile tile, out bool executeOnVisit)
    {
        executeOnVisit = true;

        var existingPlacement = tile.Placement;
        if (existingPlacement == null)
            return;

        var relevantAction = existingPlacement.Actions
            .FirstOrDefault(a => a.CanPayCost(card));
        if (relevantAction != null)
        {
            if (relevantAction.Upgrade != null)
            {
                tile.SpawnPlacement(relevantAction.Upgrade);
            }

            if (relevantAction.OnUpgrade != null)
            {
                relevantAction.OnUpgrade.DoWork(this, existingPlacement, card);
            }

            if (relevantAction.SkipOnVisit)
            {
                executeOnVisit = false;
            }

            if (card != null)
            {
                card.transform.SetParent(null);
                GameObject.Destroy(card.gameObject);
            }
        }
    }

    void OnVisit(Tile tile)
    {
        var placement = tile.Placement;
        if (placement == null)
            return;
        placement.Visited(this, placement);
    }
}
