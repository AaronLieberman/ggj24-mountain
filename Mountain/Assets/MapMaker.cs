using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    public Placement AirlockPlacement;

    public void MakeMap()
    {
        var map = Utilities.GetRootComponent<TileGridLayout>();
        


        map.GetTileFromLoc(new Vector2Int(map.HomeLocation.x+1, map.HomeLocation.x)).SpawnPlacement(AirlockPlacement);

    }
}
