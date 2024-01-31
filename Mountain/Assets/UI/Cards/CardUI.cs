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

    public bool InUse { get; private set; }

    public void SetSelected(bool selected)
    {
        SelectedBackground.SetActive(selected);
    }

    public void SetTexture()
    {
        if (Card.PlacementToSpawn == null)
        {
            CardRenderer.sprite = null;
            CardRenderer.color = Color.white;
            return;
        }

        var cardSprite = Card.IsRevealed
            ? (Card.PlacementToSpawn.CardSprite != null
                ? Card.PlacementToSpawn.CardSprite
                : (Card.PlacementToSpawn.Biome != null ? Card.PlacementToSpawn.Biome.CardSprite : null))
            : (Card.PlacementToSpawn.Biome != null && Card.PlacementToSpawn.Biome.CardSprite != null
                ? Card.PlacementToSpawn.Biome.CardSprite
                : Card.PlacementToSpawn.CardSprite);
        CardRenderer.sprite = cardSprite;
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
        var placement = Card.IsRevealed ? Card.PlacementToSpawn : (Card.PlacementToSpawn != null ? Card.PlacementToSpawn.Biome : null);
        if (placement != null)
        {
            Utilities.GetRootComponent<GameManager>().InvokeShowTooltip(placement);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Utilities.GetRootComponent<GameManager>().InvokeHideTooltip();
    }
}
