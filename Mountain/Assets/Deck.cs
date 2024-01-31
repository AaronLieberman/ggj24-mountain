using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InitialCard
{
    public Placement Placement;
    public bool IsRevealed;
}

public class Deck : MonoBehaviour
{
    public List<InitialCard> InitialCards;

    public void Reset()
    {
        Utilities.DestroyAllChildren(transform);

        foreach (var initialCard in InitialCards)
        {
            var cardObject = new GameObject();
            var card = cardObject.AddComponent<Card>();
            card.PlacementToSpawn = initialCard.Placement;
            card.IsRevealed = initialCard.IsRevealed;
            card.transform.SetParent(transform);
        }
    }

    public Card GetTopCard()
    {
        if (transform.childCount == 0) return null;

        return transform.GetChild(0).GetComponent<Card>();
    }
}
