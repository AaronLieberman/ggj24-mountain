using UnityEngine;
using UnityEditor;
using System.IO;
using System.ComponentModel;
using System;

public class ImportCards : EditorWindow
{
    private static string CardsCSVPath = "/Editor/CSVs/Cards.csv";

    public const int TILENAME = 2;
    public const int BIOME = 3;
    public const int CHANCETOLOST = 4;
    public const int DIFFICULTY = 5;
    public const int IMPASSABLE = 5;
    public const int FLAVORTEXT = 7;


    [MenuItem("I Made An Editor Tool For A Game Jam Game/Import Cards")]
    public static void DoCardImport()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + CardsCSVPath);
        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            string prefabName = splitData[TILENAME];

            if (splitData[TILENAME] == "Name")
            {
                //This is the columns title row
                Debug.Log("Skipping CSV row " + GetPrefabPath(prefabName));

                continue;
            }
            if (splitData[TILENAME] == "")
            {
                //This is a blank row
                Debug.Log("Skipping blank CSV row");
                continue;
            }



            Debug.Log("Filling prefab: " + GetPrefabPath(prefabName));


            GameObject prefabTofill = FindOrCreatePrefab(prefabName);

            // Save the new GameObject as a prefab
            GameObject prefabInstance = PrefabUtility.SaveAsPrefabAsset(prefabTofill, GetPrefabPath(prefabName));

            Placement tilePlacement = prefabInstance.GetComponent<Placement>();
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
            if(splitData[IMPASSABLE] == "TRUE")
            {
                tilePlacement.PathingHeuristic = 0.0f;
            }
            else
            {
                tilePlacement.PathingHeuristic = 1.0f;
            }


            // Destroy the instantiated GameObject
            DestroyImmediate(prefabTofill);

            Debug.Log("Prefab created at: " + GetPrefabPath(prefabName));


        }
        Debug.Log("Finished importing cards!!!");

    }

    protected static GameObject FindOrCreatePrefab(string PrefabName)
    {
        Vector3 spawnPosition = Vector3.zero;
        // TODO
        if(false)
        {
            //Prefab exists
           

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

    protected static string GetPrefabPath(string prefabName)
    {
        return AssetDatabase.GenerateUniqueAssetPath("Assets/Placements/" + prefabName + ".prefab");
    }
}