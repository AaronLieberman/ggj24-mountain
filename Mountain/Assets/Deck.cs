using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> InitialCards;

    public void Reset()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var card in InitialCards)
        {
            Instantiate(card, transform);
        }
    }

    public Card GetTopCard()
    {
        if (transform.childCount == 0) return null;

        return transform.GetChild(0).GetComponent<Card>();
    }
}
