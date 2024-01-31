using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllTerrain : MonoBehaviour
{

    public Placement ConnectedPlacement;

    public GameObject FieldsTerrain;
    public GameObject WoodsTerrain;
    public GameObject SwampTerrain;
    public GameObject SettlementTerrain;
    public GameObject WastelandTerrain;

    public GameObject ErrorTile;

    public string BiomeSetAs;

    public void UpdateBiome(String Biome)
    {
        BiomeSetAs = Biome;
        switch (Biome)
        {
            case "Fields":
                FieldsTerrain.SetActive(true); break;
            case "Woods":
                WoodsTerrain.SetActive(true); break;
            case "Swamp":
                SwampTerrain.SetActive(true); break;
            case "Settlement":
                SettlementTerrain.SetActive(true); break;
            case "Wasteland":
                WastelandTerrain.SetActive(true); break;
            default:
                ErrorTile.SetActive(true);
                Debug.Log("A biome that the AllTerrain doesn't recognize was passed in. It was: " + Biome);
                break;
        }
    }
}
