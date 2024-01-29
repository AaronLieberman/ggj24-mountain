using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MapMaker : MonoBehaviour
{
    public Placement AirlockPlacement;
    public Vector2Int AirlockOffset = new Vector2Int(1, 0);
    public Placement Videonarium;
    public Vector2Int VidonariumOffset = new Vector2Int(1,-1);
    public Placement DatabaseProteinPrinter;
    public Vector2Int DatabaseProteinPrinterOffset = new Vector2Int(1,1);
    public Placement TheGreatGate;
    public Vector2Int TheGreatGateOffset = new Vector2Int(1,1);
    public Placement PurificationPlantMaintenanceSlough;
    public Vector2Int PurificationPlantMaintenanceSloughOffset = new Vector2Int(1,1);
    //public Placement Plasticom; //Plasticom is the center slot
    public Placement ComplexFoundation;
    public Vector2Int ComplexFoundationOffset = new Vector2Int(1,1);
    
    public Vector2Int[] UnpassableTiles;

    public Placement Unpassable;

    public void MakeMap()
    {
        var map = Utilities.GetRootComponent<TileGridLayout>();

        map.GetTileFromLoc(map.HomeLocation + AirlockOffset).SpawnPlacement(AirlockPlacement);
        map.GetTileFromLoc(map.HomeLocation + DatabaseProteinPrinterOffset).SpawnPlacement(DatabaseProteinPrinter);
        map.GetTileFromLoc(map.HomeLocation + TheGreatGateOffset).SpawnPlacement(TheGreatGate);
        map.GetTileFromLoc(map.HomeLocation + PurificationPlantMaintenanceSloughOffset).SpawnPlacement(PurificationPlantMaintenanceSlough);
        map.GetTileFromLoc(map.HomeLocation + ComplexFoundationOffset).SpawnPlacement(ComplexFoundation);
        map.GetTileFromLoc(map.HomeLocation + VidonariumOffset).SpawnPlacement(Videonarium);


        foreach (var tile in UnpassableTiles)
        {
            map.GetTileFromLoc(tile).SpawnPlacement(Unpassable);

        }




    }
}
