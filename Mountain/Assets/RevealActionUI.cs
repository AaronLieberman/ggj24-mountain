using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RevealActionUI : MonoBehaviour
{
    [SerializeField] private GameObject revealActionUI;
    [SerializeField] private TMP_Text BodyText;

    void Awake()
    {
        Utilities.GetRootComponent<GameManager>().ShowOnRevealedUI += (_, text) => ShowOnRevealedUI(text);
    }

    void ShowOnRevealedUI(string text)
    {
        Utilities.GetRootComponent<GameManager>().IsDialogVisible = true;
        revealActionUI.SetActive(true);
        BodyText.text = text;
    }

    public void AcceptRevealedWork()
    {
        Utilities.GetRootComponent<GameManager>().IsDialogVisible = false;
        revealActionUI.SetActive(false);
    }
}
