using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public int MaxHandSize;

    public Deck Deck { get; set; }

    public HandUI HandUI;

    public void Reset()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        HandUI.RefreshHandUI();
    }

    public bool DrawTillFull()
    {
        for (int i = transform.childCount; i < MaxHandSize; i++)
        {
            var card = Deck.GetTopCard();
            if (card == null) return false;

            card.transform.parent = transform;
        }

        HandUI.RefreshHandUI();

        return true;
    }
}