using UnityEngine;
using UnityEditor;
using System.IO;
using System.ComponentModel;
using System;

public class ImportCards : EditorWindow
{
    private static string CardsCSVPath = "/Editor/CSVs/Cards.csv";

    [MenuItem("I Made An Editor Tool For A Game Jam Game/Import Cards")]
    public static void DoCardImport()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + CardsCSVPath);
        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            string prefabName = "NewPrefab";
            
            GameObject prefabTofill = FindOrCreatePrefab(prefabName);

            // Create the prefab path
            string prefabPath = "Assets/Placements/" + prefabName + ".prefab";
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);

            // Save the new GameObject as a prefab
            GameObject prefabInstance = PrefabUtility.SaveAsPrefabAsset(prefabTofill, prefabPath);

            // Destroy the instantiated GameObject
            DestroyImmediate(prefabTofill);

            Debug.Log("Prefab created at: " + prefabPath);
        }
    }

    protected static GameObject FindOrCreatePrefab(string PrefabName)
    {
        Vector3 spawnPosition = Vector3.zero;
        // Instantiate a new GameObject
        GameObject newObject = new GameObject(PrefabName);
        newObject.transform.position = spawnPosition;
        return newObject;
    }
}
