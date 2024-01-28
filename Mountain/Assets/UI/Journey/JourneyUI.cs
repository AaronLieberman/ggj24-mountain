using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JourneyUI : MonoBehaviour
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
        foreach (Transform child in JourneySectionTransform)
        {
            Destroy(child.gameObject);
        }

        List<WorkerPlan> workerPlan = Utilities.GetRootComponent<GameManager>().WorkerPlan;
        for (int i = 0; i < Utilities.GetRootComponent<GameManager>().MaxCards; i++)
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
    }

    public void OnPressGoPlanningUI()
    {
         Utilities.GetRootComponent<GameManager>().StartWorkerOnJourney();
    }

    void RefreshDeckAvailabilityUI()
    {
        JourneyResetButton.interactable = Utilities.GetRootComponent<GameManager>().IsWorkerAvailable;
        JourneyGoButton.interactable = Utilities.GetRootComponent<GameManager>().IsWorkerAvailable;
    }
}
