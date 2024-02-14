using UnityEngine;

public class DEBUGPlaceAdjacentRandomSwampAction : PlacementAction
{
    public PlaceAdjacentTile PlaceAdjacentTile;
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        PlacementPoolManager ppm = Utilities.GetRootComponent<PlacementPoolManager>();
        PlaceAdjacentTile.TileToPlace = ppm.GetRandomCardFromBiome(Utilities.GetRootComponent<Deck>().SwampBiome);
        PlaceAdjacentTile.DoWork(worker, placement, card);
    }

}
