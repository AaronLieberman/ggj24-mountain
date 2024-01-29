using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RevealActionUI : MonoBehaviour
{
    [SerializeField] private GameObject revealActionUI;

    void Awake()
    {
        Utilities.GetRootComponent<GameManager>().ShowWorkRevealedUI += (_, __) => ShowWorkRevealedUI();
    }

    void ShowWorkRevealedUI()
    {
        Utilities.GetRootComponent<GameManager>().IsWorkRevealed = true;
        revealActionUI.SetActive(true);
    }

    public void AcceptRevealedWork()
    {
        Utilities.GetRootComponent<GameManager>().IsWorkRevealed = false;
        revealActionUI.SetActive(false);
    }
}
