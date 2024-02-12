using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllTerrain : MonoBehaviour
{

    public Placement ConnectedPlacement;

    public GameObject FieldsTerrain;
    public GameObject WoodsTerrain;
    public GameObject SettlementTerrain;
    public GameObject SwampTerrain;
    public GameObject WastelandTerrain;
    public GameObject MountaintopTerrain;

    public GameObject ErrorTile;

    public string BiomeSetAs;

    public void UpdateBiome()
    {
        ConnectedPlacement = GetComponentInParent<Placement>().Biome;
        BiomeSetAs = ConnectedPlacement.Name;

        FieldsTerrain.SetActive(false);
        WoodsTerrain.SetActive(false);
        SwampTerrain.SetActive(false);
        SettlementTerrain.SetActive(false);
        WastelandTerrain.SetActive(false);
        MountaintopTerrain.SetActive(false);

        switch (ConnectedPlacement.Name)
        {
            case "Fields":
                ErrorTile.SetActive(false);
                FieldsTerrain.SetActive(true); 
                break;
            case "Woods":
                ErrorTile.SetActive(false);
                WoodsTerrain.SetActive(true); 
                break;
            case "Swamp":
                ErrorTile.SetActive(false);
                SwampTerrain.SetActive(true); 
                break;
            case "Settlement":
                ErrorTile.SetActive(false);
                SettlementTerrain.SetActive(true); 
                break;
            case "Wasteland":
                ErrorTile.SetActive(false);
                WastelandTerrain.SetActive(true); 
                break;
            case "Mountaintop":
                ErrorTile.SetActive(false);
                WastelandTerrain.SetActive(true); 
                break;
            case "zzDebug":
                ErrorTile.SetActive(true);
                break;
            default:
                ErrorTile.SetActive(true);
                Debug.Log("A biome that the AllTerrain doesn't recognize was selected. It was: " + BiomeSetAs);
                break;
        }
    }

    public void Start()
    {
        UpdateBiome();
    }
}
