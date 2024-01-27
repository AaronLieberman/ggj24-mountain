using UnityEngine;

public class DeckUI : MonoBehaviour
{
    [SerializeField] private GameObject DeckSlotPrefab;
    [SerializeField] private Transform CardsSectionTransform;

    public void RefreshDeckUI()
    {
        foreach (Transform child in CardsSectionTransform)
        {
            Destroy(child.gameObject);
        }

        /*
        Deck deck = Utilities.GetRootComponent<Deck>();
        foreach (Transform child in deck.transform)
        {
            GameObject cardSlot = Instantiate(DeckSlotPrefab, CardsSectionTransform);
            cardSlot.GetComponent<CardUI>().Card = child.GetComponent<Card>();
            
            cardSlot.GetComponent<CardUI>().SetTexture();
        }
        */

        Deck deck = Utilities.GetRootComponent<Deck>();
        for (int i = deck.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = deck.transform.GetChild(i);
            GameObject cardSlot = Instantiate(DeckSlotPrefab, CardsSectionTransform);
            cardSlot.GetComponent<CardUI>().Card = child.GetComponent<Card>();
            
            cardSlot.GetComponent<CardUI>().SetTexture();
        }
    }
}
