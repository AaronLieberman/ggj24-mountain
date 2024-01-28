using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HandUI : MonoBehaviour
{

    [SerializeField] private GameObject CardSlotPrefab;
    [SerializeField] private Transform CardSectionTransform;

    void Awake()
    {
        Utilities.GetRootComponent<Hand>().HandChanged += (_, __) => RefreshHandUI();
    }

    void RefreshHandUI()
    {
        foreach (Transform child in CardSectionTransform)
        {
            Destroy(child.gameObject);
        }

        Hand hand = Utilities.GetRootComponent<Hand>();
        foreach (Transform child in hand.transform)
        {
            GameObject cardSlot = Instantiate(CardSlotPrefab, CardSectionTransform);
            cardSlot.GetComponent<CardUI>().Card = child.GetComponent<Card>();

            cardSlot.GetComponent<CardUI>().SetTexture();
        }
    }
}