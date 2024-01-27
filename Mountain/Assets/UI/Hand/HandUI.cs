using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    
    [SerializeField] private GameObject cardSlotPrefab;
    [SerializeField] private Transform cardSectionTransform;

    public void RefreshHandUI()
    {
        foreach (Transform child in cardSectionTransform)
        {
            Destroy(child.gameObject);
        }

        Hand hand = Utilities.GetRootComponent<Hand>();
        foreach (Transform child in hand.transform)
        {
            Instantiate(cardSlotPrefab, cardSectionTransform);
        }
    }
}
