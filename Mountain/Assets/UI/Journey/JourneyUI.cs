using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JourneyUI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private GameObject JourneySlotPrefab;
    [SerializeField] private Transform JourneySectionTransform;
    [SerializeField] private Button JourneyResetButton;
    [SerializeField] private Button JourneyGoButton;

    void Awake()
    {
        Utilities.GetRootComponent<GameManager>().WorkerAvailableChanged += (_, __) => RefreshDeckAvailabilityUI();
        Utilities.GetRootComponent<GameManager>().WorkerPlanChanged += (_, __) => RefreshJourneyPlanningUI();
    }

    void RefreshJourneyPlanningUI()
    {
        Utilities.DestroyAllChildren(JourneySectionTransform.transform);

        List<WorkerPlan> workerPlan = Utilities.GetRootComponent<GameManager>().WorkerPlan;
        for (int i = 0; i < Utilities.GetRootComponent<GameManager>().MaxJourneySlots; i++)
        {
            GameObject journeySlot = Instantiate(JourneySlotPrefab, JourneySectionTransform);

            if (workerPlan.Count >= i + 1)
            {
                journeySlot.GetComponent<CardUI>().Card = Utilities.GetRootComponent<GameManager>().WorkerPlan[i].Card;
                journeySlot.GetComponent<CardUI>().SetTexture();
            }
        }
    }

    public void OnPressResetPlanningUI()
    {
        Utilities.GetRootComponent<GameManager>().ClearWorkerPlan();
        ResetHandSelectionContext();
    }

    public void OnPressGoPlanningUI()
    {
         Utilities.GetRootComponent<GameManager>().StartWorkerOnJourney();
         ResetHandSelectionContext();
    }

    void ResetHandSelectionContext()
    {
        Utilities.GetRootComponent<Canvas>().GetComponentInChildren<HandUI>().SetSelectedCardUI(null);
    }

    void RefreshDeckAvailabilityUI()
    {
        JourneyResetButton.interactable = Utilities.GetRootComponent<GameManager>().IsWorkerAvailable;
        JourneyGoButton.interactable = Utilities.GetRootComponent<GameManager>().IsWorkerAvailable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Utilities.GetRootComponent<GameManager>().InvokeHideTooltip();
    }
}
