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
        // Set general tooltip attributes section.
        var cell = placement.transform.parent != null
            ? (Vector2Int?)Utilities.GetRootComponent<Grid>().LocalToCell(placement.transform.parent.localPosition)
            : null;
        tooltipPlacementNameText.text = placement.Name + (cell != null ? $" ({cell.Value.x}, {cell.Value.y})" : "");
        tooltipPlacementDescriptionText.text = placement.FlavorText;
        tooltipPlacementLostChanceText.text = String.Format("Chance to get lost: {0}%", placement.LostChance * 100);
        tooltipPlacementPassableText.text = placement.PathingHeuristic > 10000 ? "Impassable" : "Passable";

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
        foreach (Transform child in tooltipPlacementOnVisitsSection)
        {
            Destroy(child.gameObject);
        }

        GameObject visitObject = Instantiate(tooltipPlacementOnVisitPrefab, tooltipPlacementOnVisitsSection);
        visitObject.GetComponent<TooltipOnVisitUI>().OnVisitText.text = placement.OnVisitText;
        visitObject.GetComponent<TooltipOnVisitUI>().OnVisitAddText.text = String.Format("Add {0} to your deck", placement.OnVisitText);
    }
}
