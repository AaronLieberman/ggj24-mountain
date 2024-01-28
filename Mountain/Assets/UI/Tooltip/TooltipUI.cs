using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private GameObject tooltipContentSection;

    [SerializeField] private Vector2 tooltipOffsetFactor;
    
    [SerializeField] private TextMeshProUGUI tooltipPlacementNameText;
    [SerializeField] private TextMeshProUGUI tooltipPlacementDescriptionText;
    [SerializeField] private TextMeshProUGUI tooltipPlacementLostChanceText;
    [SerializeField] private TextMeshProUGUI tooltipPlacementPassableText;
    [SerializeField] private GameObject tooltipPlacementAbilityPrefab;
    [SerializeField] private Transform tooltipPlacementAbilitiesSection;
    [SerializeField] private GameObject tooltipPlacementOnVisitPrefab;
    [SerializeField] private Transform tooltipPlacementOnVisitsSection;

    void Awake()
    {
        Utilities.GetRootComponent<GameManager>().ShowTooltip += (_, placement) => ShowTooltipUI(placement);
        Utilities.GetRootComponent<GameManager>().HideTooltip += (_, __) => HideTooltipUI();
    }

    void ShowTooltipUI(Placement placement)
    {
        tooltipContentSection.transform.position = new Vector3(
            Input.mousePosition.x + (tooltipOffsetFactor.x * Screen.width), 
            Input.mousePosition.y + (tooltipOffsetFactor.y * Screen.height), 
            Input.mousePosition.z
        );

        RefreshTooltipUIContent(placement);

        tooltipContentSection.SetActive(true);
    }

    void HideTooltipUI()
    {
        tooltipContentSection.SetActive(false);
    }

    void RefreshTooltipUIContent(Placement placement)
    {
        tooltipPlacementNameText.text = placement.Name;
        tooltipPlacementDescriptionText.text = placement.FlavorText;
        tooltipPlacementLostChanceText.text = String.Format("Chance to get lost: {0}%", (placement.LostChance * 100));
        
    }
}
