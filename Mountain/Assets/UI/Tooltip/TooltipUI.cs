using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        PositionTooltipUI();

        RefreshTooltipUIContent(placement);

        tooltipContentSection.SetActive(true);
    }

    void HideTooltipUI()
    {
        tooltipContentSection.SetActive(false);
    }

    void PositionTooltipUI()
    {
        // If tooltip can fit on the right side of the cursor, position X as such else position it to the left.
        if ((Input.mousePosition.x + (tooltipOffsetFactor.x * Screen.width) + tooltipContentSection.GetComponent<RectTransform>().rect.width) < Screen.width)
            tooltipContentSection.transform.position = new Vector3(Input.mousePosition.x + (tooltipOffsetFactor.x * Screen.width), tooltipContentSection.transform.position.y, tooltipContentSection.transform.position.z);
        else
            tooltipContentSection.transform.position = new Vector3(Input.mousePosition.x - (tooltipOffsetFactor.x * Screen.width), tooltipContentSection.transform.position.y, tooltipContentSection.transform.position.z);

        // If tooltip can fit on the top side of the cursor, position Y as such else position it under.
        if ((Input.mousePosition.y + (tooltipOffsetFactor.y * Screen.height) + tooltipContentSection.GetComponent<RectTransform>().rect.height) < Screen.height)
            tooltipContentSection.transform.position = new Vector3(tooltipContentSection.transform.position.x, Input.mousePosition.y + (tooltipOffsetFactor.y * Screen.height), tooltipContentSection.transform.position.z);
        else
            tooltipContentSection.transform.position = new Vector3(tooltipContentSection.transform.position.x, Input.mousePosition.y - (tooltipOffsetFactor.y * Screen.height), tooltipContentSection.transform.position.z);
    }

    void RefreshTooltipUIContent(Placement placement)
    {
        var distances = PathFinder.CalculateUnexploredDistance(Utilities.GetRootComponent<TileGridLayout>().HomeLocation, Utilities.GetRootComponent<GameManager>().MaxJourneySlots);

        // Set general tooltip attributes section.

        if (Utilities.GetRootComponent<GameManager>().EnableDebugTools == true)
        {
            var cell = placement.transform.parent != null
                ? (Vector2Int?)Utilities.GetRootComponent<Grid>().LocalToCell(placement.transform.parent.localPosition)
                : null;
            tooltipPlacementNameText.text = placement.Name + (cell != null ? $" ({cell.Value.x}, {cell.Value.y})" : "");
            tooltipPlacementPassableText.text = (placement.PathingHeuristic >= 10000 ? "Impassable" : "Passable") + (cell.HasValue && distances.ContainsKey(cell.Value) ? (distances[cell.Value].Passable ? " passable: " + distances[cell.Value].UnexploredDistance : " impassible") : -1);

        }
        else
        {
            tooltipPlacementNameText.text = placement.Name;
            tooltipPlacementPassableText.text = placement.PathingHeuristic >= 10000 ? "Impassable" : "Passable";
        }
        tooltipPlacementDescriptionText.text = placement.FlavorText;
        tooltipPlacementLostChanceText.text = String.Format("Chance to get lost: {0}%", placement.LostChance * 100);

        // Refresh tooltip ability UI section.
        Utilities.DestroyAllChildren(tooltipPlacementAbilitiesSection);

        foreach (TileAction tileAction in placement.Actions)
        {
            GameObject abilityObject = Instantiate(tooltipPlacementAbilityPrefab, tooltipPlacementAbilitiesSection);
            abilityObject.GetComponent<TooltipAbilityUI>().AbilityCostText.text = tileAction.Cost != null
                ? tileAction.Cost.Name
                : "Any";
            abilityObject.GetComponent<TooltipAbilityUI>().AbilityUpgradeToText.text = tileAction.Upgrade != null
                ? String.Format("Upgrade to {0}", tileAction.Upgrade.Name)
                : "Unknown";
        }

        // Refresh tooltip OnVisit UI section.
        Utilities.DestroyAllChildren(tooltipPlacementOnVisitsSection.transform);

        GameObject visitObject = Instantiate(tooltipPlacementOnVisitPrefab, tooltipPlacementOnVisitsSection);
        visitObject.GetComponent<TooltipOnVisitUI>().OnVisitDescriptionText.text = placement.OnVisitTooltipText;
    }
}
