using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RevealActionUI : MonoBehaviour
{
    [SerializeField] private GameObject revealActionUI;
    [SerializeField] private TMP_Text TitleText;
    [SerializeField] private TMP_Text BodyText;

    void Awake()
    {
        Utilities.GetRootComponent<GameManager>().ShowOnRevealedUI += (_, text) => ShowOnRevealedUI(text);
    }

    void ShowOnRevealedUI(PopupText popupText)
    {
        Utilities.GetRootComponent<GameManager>().IsDialogVisible = true;
        revealActionUI.SetActive(true);
        if (TitleText != null)
        {
            TitleText.text = popupText.Title ?? "";
        }

        if (BodyText != null)
        {
            BodyText.text = popupText.Body;
        }
    }

    public void AcceptRevealedWork()
    {
        Utilities.GetRootComponent<GameManager>().IsDialogVisible = false;
        revealActionUI.SetActive(false);
    }
}
