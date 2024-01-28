using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyUI : MonoBehaviour
{
    [SerializeField] private GameObject JourneySlotPrefab;
    [SerializeField] private Transform JourneySectionTransform;

    void RefreshJourneyPlanningUI()
    {
        foreach (Transform child in JourneySectionTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnPressResetPlanningUI()
    {
        Debug.Log("Reset planning setup.");
    }

    public void OnPressGoPlanningUI()
    {
         Debug.Log("Play planning setup.");
    }
}
