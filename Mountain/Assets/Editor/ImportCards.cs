using UnityEngine;
using UnityEditor;
using System.IO;
using System.ComponentModel;
using System;
using System.Collections.Generic;

public class ImportCards : EditorWindow
{
    private static string CardsCSVPath = "/Editor/CSVs/Cards.tsv";

    public const int PRIORITY = 0;
    public const int CANBEDRAWN = 1;
    public const int TILENAME = 2;
    public const int BIOME = 3;
    public const int CHANCETOLOST = 4;
    public const int DIFFICULTY = 5;
    public const int IMPASSABLE = 6;
    public const int FLAVORTEXT = 7;
    public const int ABILITY = 8;
    public const int SECRETEFFECT = 9;
    public const int ONREVEAL = 10;
    public const int ONREVEALTEXT = 11;
    public const int ONVISIT = 12;
    public const int ONVISITTEXT = 13;
    public const int POPUPTEXT = 14;
    public const int POOL1 = 16;
    public const int POOL2 = 17;
    public const int POOL3 = 18;
    public const int POOL4 = 19;


    [MenuItem("I Made An Editor Tool For A Game Jam Game/Import Cards")]
    public static void DoCardImport()
    {
        //initialize cardPool
        CardPool cardPool1 = AssetDatabase.LoadAssetAtPath<CardPool>("Assets/CardPool/CardPool1.asset");
        CardPool cardPool2 = AssetDatabase.LoadAssetAtPath<CardPool>("Assets/CardPool/CardPool2.asset");
        CardPool cardPool3 = AssetDatabase.LoadAssetAtPath<CardPool>("Assets/CardPool/CardPool3.asset");
        CardPool cardPool4 = AssetDatabase.LoadAssetAtPath<CardPool>("Assets/CardPool/CardPool4.asset");

        //clear old list
        cardPool1.tilePlacementObjects = new List<GameObject>();
        cardPool2.tilePlacementObjects = new List<GameObject>();
        cardPool3.tilePlacementObjects = new List<GameObject>();
        cardPool4.tilePlacementObjects = new List<GameObject>();

        CardPool[] cardPools = { cardPool1, cardPool2, cardPool3, cardPool4 };

        //parse csv
        string[] allLines = File.ReadAllLines(Application.dataPath + CardsCSVPath);
        foreach (string s in allLines)
        {
            string[] splitData = s.Split('\t');

            string prefabName = splitData[TILENAME];

            if (splitData[TILENAME] == "Name")
            {
                //This is the columns title row
                Debug.Log("Skipping CSV row " + GetPlacementPath(prefabName));

                continue;
            }
            if (splitData[TILENAME] == "")
            {
                //This is a blank row
                Debug.Log("Skipping blank CSV row");
                continue;
            }


            Debug.Log("Filling prefab: " + GetPlacementPath(prefabName));


            GameObject prefabTofill = FindOrCreatePrefab(prefabName);

            // // Save the new GameObject as a prefab
            //GameObject prefabInstance = PrefabUtility.SaveAsPrefabAsset(prefabTofill, GetPrefabPath(prefabName));

            //Placement tilePlacement = prefabInstance.GetComponent<Placement>();
            Placement tilePlacement = prefabTofill.GetComponent<Placement>();


            if (splitData[CHANCETOLOST] == "")
            {
                tilePlacement.LostChance = 0.05f;
            }
            else
            {
                tilePlacement.LostChance = float.Parse(splitData[CHANCETOLOST]);
            }
            if (splitData[DIFFICULTY] == "")
            {
                tilePlacement.Difficulty = int.MaxValue;
            }
            else
            {
                tilePlacement.Difficulty = int.Parse(splitData[DIFFICULTY]);
            }
            tilePlacement.FlavorText = splitData[FLAVORTEXT];
            tilePlacement.Name = splitData[TILENAME];
            if (splitData[IMPASSABLE] == "TRUE")
            {
                tilePlacement.PathingHeuristic = 100000.0f;
            }
            else
            {
                tilePlacement.PathingHeuristic = 1.0f;
            }

            GameObject prefab = GetPlacementPrefabFromName(splitData[BIOME]);
            Placement prefabPlacement = prefab.GetComponent<Placement>();
            if (prefabPlacement == null)
            {
                Debug.LogError("No Placement component exists on " + prefab.name);
            }
            tilePlacement.Biome = prefabPlacement;
            tilePlacement.OnRevealText = splitData[ONREVEALTEXT];
            tilePlacement.OnVisitTooltipText = splitData[ONVISITTEXT];

            UpdateAllTerrainGameObject(tilePlacement, splitData[BIOME]);

            if(!string.IsNullOrEmpty(splitData[POPUPTEXT]))
            {
                ShowPopupText popupcomponent = prefabTofill.GetComponent<ShowPopupText>();
                if(popupcomponent == null) 
                {
                    popupcomponent = prefabTofill.AddComponent<ShowPopupText>();
                }
                popupcomponent.textToPopUp = splitData[POPUPTEXT];
            }

            //AssetDatabase.DeleteAsset(GetPrefabPath(prefabName));
            // Save the new GameObject as a prefab
            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(prefabTofill, GetPlacementPath(prefabName));

            //populate cardPools
            for (int i = 0; i < cardPools.Length; i++)
            {
                // Check if the current row belongs to the current card pool
                if (splitData[POOL1 + i] == "1")
                {
                    cardPools[i].tilePlacementObjects.Add(savedPrefab);
                }
            }

            // Destroy the instantiated GameObject
            DestroyImmediate(prefabTofill);

            Debug.Log("Prefab created at: " + GetPlacementPath(prefabName));
        }
        Debug.Log("Finished importing cards!!!");

    }

    private static void UpdateAllTerrainGameObject(Placement tilePlacement, string BiomeText)
    {
        AllTerrain aT = tilePlacement.GetComponentInChildren<AllTerrain>();
        if (aT == null)
        {
            Debug.Log("Adding AllTerrainComponent to " + tilePlacement.Name);
            GameObject AllTerrainPrefab = GetPrefabFromName("AllTerrain");
            GameObject newAllTerrain = Instantiate(AllTerrainPrefab);
            newAllTerrain.transform.SetParent(tilePlacement.transform);
            aT = newAllTerrain.GetComponent<AllTerrain>();

        }
        aT.UpdateBiome();
    }

    protected static GameObject FindOrCreatePrefab(string PrefabName)
    {
        Vector3 spawnPosition = Vector3.zero;
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetPlacementPath(PrefabName));
        if (existingPrefab != null)
        {
            GameObject prefabInWorld = Instantiate(existingPrefab);
            //Prefab exists
            return prefabInWorld;
        }
        else
        {
            // Instantiate a new GameObject
            GameObject newObject = new GameObject(PrefabName);
            newObject.transform.position = spawnPosition;
            newObject.AddComponent<Placement>();
            return newObject;
        }
    }

    protected static string GetPlacementPath(string prefabName)
    {
        return "Assets/Placements/" + prefabName + ".prefab";
    }

    protected static GameObject GetPlacementPrefabFromName(string prefabName)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetPlacementPath(prefabName));
        if (prefab == null)
        {
            Debug.LogError("No placement prefab exists at " + GetPlacementPath(prefabName));
        }
        return prefab;
    }

    protected static GameObject GetPrefabFromName(string prefabName)
    {

        string prefabPath = "Assets/Prefabs/" + prefabName + ".prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("No prefab exists at " + prefabPath);
        }
        return prefab;
    }


}
