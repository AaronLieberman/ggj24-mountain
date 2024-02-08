using UnityEngine;
using UnityEngine.EventSystems;

public class DeckUI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private GameObject DeckSlotPrefab;
    [SerializeField] private Transform CardsSectionTransform;

    void Awake()
    {
        Utilities.GetRootComponent<Hand>().DeckChanged += (_, __) => RefreshDeckUI();
        Utilities.GetRootComponent<Deck>().DeckChanged += (_, __) => RefreshDeckUI();
    }

    void RefreshDeckUI()
    {
        Utilities.DestroyAllChildren(CardsSectionTransform);

        Deck deck = Utilities.GetRootComponent<Deck>();
        for (int i = deck.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = deck.transform.GetChild(i);
            GameObject cardSlot = Instantiate(DeckSlotPrefab, CardsSectionTransform);
            cardSlot.GetComponent<CardUI>().Card = child.GetComponent<Card>();
            
            cardSlot.GetComponent<CardUI>().SetTexture();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Utilities.GetRootComponent<GameManager>().InvokeHideTooltip();
    }
}
