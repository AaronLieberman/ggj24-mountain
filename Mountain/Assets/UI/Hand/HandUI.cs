using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public GameObject cardUIPrefab;

    private Transform cardSectionTransform;

    public void Awake()
    {
        cardSectionTransform = transform.Find("CardsSection");
    }

    public void RefreshHandUI()
    {
        // Clear hand UI objects
        foreach (Transform child in cardSectionTransform)
        {
            Destroy(child.gameObject);
        }

        
    }
}
