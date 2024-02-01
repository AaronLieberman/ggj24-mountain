using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject GameOverContent;

    private void Start()
    {
        GameOverContent.SetActive(false);
    }

    public void ShowGameOverUI()
    {
        GameOverContent.SetActive(true);
    }
}
