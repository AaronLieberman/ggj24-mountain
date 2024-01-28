using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System;
using UnityEditor;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UnityEngine.UI.Image CardRenderer;
    [SerializeField] private GameObject SelectedBackground;


    public Card Card { get; set; }

    public bool InUse{get;private set;}

    public void SetSelected(bool selected)
    {
        SelectedBackground.SetActive(selected);
    }

    public void SetTexture()
    {
        CardRenderer.sprite = Card.CardDetails.CardSprite;
        Color existingTextureColor = CardRenderer.color;
        existingTextureColor.a = 1.0f;
        CardRenderer.color = existingTextureColor;
    }

    public void SetInUse(bool inUse)
    {
        InUse = inUse;
        
        var handUI = Utilities.GetRootComponents<Canvas>()
            .Select(c => c.GetComponentInChildren<HandUI>())
            .Single();
        if (inUse && handUI.SelectedCardUI == this)
        {
            handUI.SetSelectedCardUI(null);
        }

        var button = GetComponentInChildren<UnityEngine.UI.Button>();
        var image = button.GetComponent<UnityEngine.UI.Image>();
        float s = inUse ? 0.5f : 1;
        image.color = new Color(s, s, s);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Card != null)
        {
            Utilities.GetRootComponent<GameManager>().InvokeShowTooltip(Card.IsRevealed ? Card.PlacementToSpawn : Card.UnrevealedPlacement);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Utilities.GetRootComponent<GameManager>().InvokeHideTooltip();
    }
}
