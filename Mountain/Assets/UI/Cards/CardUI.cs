using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image CardRenderer;
    [SerializeField] private GameObject SelectedBackground; 

    public Card Card { get; set; }

    public void SetSelected(bool selected)
    {
        SelectedBackground.SetActive(selected);
    }

    public void SetTexture()
    {
        CardRenderer.sprite = Card.CardDetails.CardSprite;
    }
}
