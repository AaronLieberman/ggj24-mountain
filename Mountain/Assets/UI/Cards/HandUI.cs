using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HandUI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private GameObject CardSlotPrefab;
    [SerializeField] private Transform CardSectionTransform;
    [SerializeField] private GameObject HandAvailabilityElement;

    public event Action<Card, bool> OnCardSelectStateChange = delegate { };

    void Awake()
    {
        Utilities.GetRootComponent<Hand>().HandChanged += (_, __) => RefreshHandUI();
        Utilities.GetRootComponent<GameManager>().WorkerPlanChanged += (_, __) => RefreshHandUI();
        Utilities.GetRootComponent<GameManager>().WorkerAvailableChanged += (_, __) => RefreshHandAvailabilityUI();
    }

    void RefreshHandUI()
    {
        foreach (Transform child in CardSectionTransform)
        {
            Destroy(child.gameObject);
        }

        Hand hand = Utilities.GetRootComponent<Hand>();
        List<WorkerPlan> workerPlan = Utilities.GetRootComponent<GameManager>().WorkerPlan;

        bool isPlanFull = Utilities.GetRootComponent<GameManager>().WorkerPlan.Count >= Utilities.GetRootComponent<GameManager>().MaxJourneySlots;

        foreach (Transform child in hand.transform)
        {
            GameObject cardSlot = Instantiate(CardSlotPrefab, CardSectionTransform);

            var card = child.GetComponent<Card>();
            var cardUI = cardSlot.GetComponent<CardUI>();
            cardUI.Card = card;
            cardUI.SetTexture();
            cardUI.SetInUse(isPlanFull || workerPlan.Any(a => a.Card == card));
        }
    }

    void RefreshHandAvailabilityUI()
    {
        HandAvailabilityElement.SetActive(Utilities.GetRootComponent<GameManager>().IsWorkerAvailable);
    }

    public CardUI SelectedCardUI { get; private set; }
    public void SetSelectedCardUI(CardUI cardUI)
    {
        Utilities.GetRootComponent<GameManager>().SetConsideringPlacingCard(null);

        if (cardUI != null && cardUI.InUse)
            return;

        if (Utilities.GetRootComponent<GameManager>().WorkerPlan.Count >= Utilities.GetRootComponent<GameManager>().MaxJourneySlots)
            return;

        if (SelectedCardUI?.Card != null)
            OnCardSelectStateChange.Invoke(SelectedCardUI?.Card, false);

        if (SelectedCardUI != null)
        {
            SelectedCardUI.SetSelected(false);
        }

        if (Utilities.GetRootComponent<GameManager>().IsWorkerAvailable && cardUI != null)
        {
            SelectedCardUI = cardUI;
            SelectedCardUI.SetSelected(true);
        }
        else
        {
            SelectedCardUI = null;
        }

        Utilities.GetRootComponent<GameManager>().SetConsideringPlacingCard(cardUI != null ? cardUI.Card : null);
        OnCardSelectStateChange.Invoke(SelectedCardUI?.Card, SelectedCardUI != null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Utilities.GetRootComponent<GameManager>().InvokeHideTooltip();
    }
}
