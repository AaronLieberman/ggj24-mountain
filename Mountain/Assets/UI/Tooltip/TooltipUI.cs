using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private GameObject tooltipContentSection;

    [SerializeField] private Vector2 tooltipOffset;

    void Awake()
    {
        Utilities.GetRootComponent<GameManager>().ShowTooltip += (_, __) => ShowTooltipUI();
        Utilities.GetRootComponent<GameManager>().HideTooltip += (_, __) => HideTooltipUI();
    }

    void ShowTooltipUI()
    {
        tooltipContentSection.transform.position = new Vector3(
            Input.mousePosition.x + tooltipOffset.x, 
            Input.mousePosition.y + tooltipOffset.y, 
            Input.mousePosition.z
        );
        
        tooltipContentSection.SetActive(true);
    }

    void HideTooltipUI()
    {
        tooltipContentSection.SetActive(false);
    }
}