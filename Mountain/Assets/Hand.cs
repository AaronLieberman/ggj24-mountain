using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hand : MonoBehaviour
{
    public int MaxHandSize = 3;

    public Deck Deck { get; set; }

    public event EventHandler HandChanged;
    public event EventHandler DeckChanged;

    public int GetHandCount()
    {
        return transform.childCount;
    }

    public void Reset()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        HandChanged?.Invoke(this, null);
        DeckChanged?.Invoke(this, null);
    }

    public void IncreaseHandSize()
    {
        MaxHandSize = MaxHandSize++;

        HandChanged?.Invoke(this, null);
        DeckChanged?.Invoke(this, null);
    }

    public bool DrawTillFull()
    {
        for (int i = transform.childCount; i < MaxHandSize; i++)
        {
            var card = Deck.GetTopCard();
            if (card == null)
            {
                HandChanged?.Invoke(this, null);
                return false;
            }

            card.transform.parent = transform;
        }

        HandChanged?.Invoke(this, null);
        DeckChanged?.Invoke(this, null);

        return true;
    }
}
