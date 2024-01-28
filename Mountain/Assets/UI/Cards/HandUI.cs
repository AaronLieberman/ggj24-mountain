using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] private GameObject CardSlotPrefab;
    [SerializeField] private Transform CardSectionTransform;
    [SerializeField] private GameObject HandAvailabilityElement;

    void Awake()
    {
        Utilities.GetRootComponent<Hand>().HandChanged += (_, __) => RefreshHandUI();
        Utilities.GetRootComponent<GameManager>().WorkerAvailableChanged += (_, __) => RefreshHandAvailabilityUI();
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

    void RefreshHandAvailabilityUI()
    {
        HandAvailabilityElement.SetActive(Utilities.GetRootComponent<GameManager>().IsWorkerAvailable);
    }

    public CardUI SelectedCardUI { get; private set; }
    public void SetSelectedCardUI(CardUI cardUI)
    {
        if (SelectedCardUI != null)
        {
            SelectedCardUI.SetSelected(false);
        }

        if (Utilities.GetRootComponent<GameManager>().IsWorkerAvailable && cardUI != null)
        {
            SelectedCardUI = cardUI;
            SelectedCardUI.SetSelected(true);
        }
    }
}
