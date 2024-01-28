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
    }

    void RefreshJourneyPlanningUI()
    {
        foreach (Transform child in JourneySectionTransform)
        {
            Destroy(child.gameObject);
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
