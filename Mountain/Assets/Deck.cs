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
    //We need a way to declare biome types in switch statements and stuff, but biomes are placements,
    //so put some global references to the types here so we don't have to search for the prefabs all the time.
    public Placement WoodsBiome;
    public Placement FieldsBiome;
    public Placement SettlementBiome;
    public Placement SwampBiome;
    public Placement WastelandBiome;

    public List<InitialCard> InitialCards;

    public int GetDeckCount()
    {
        return transform.childCount;
    }

    public void Reset()
    {
        Utilities.DestroyAllChildren(transform);

        foreach (var initialCard in InitialCards)
        {
            var cardObject = new GameObject();
            var card = cardObject.AddComponent<Card>();
            card.PlacementToSpawn = initialCard.Placement;
            card.IsRevealed = initialCard.IsRevealed;
			card.gameObject.name = initialCard.Placement.gameObject.name;
			card.transform.SetParent(transform);
        }
    }

    /// <summary>
    /// Brings a card from outside of the Deck back into it and onto the top.
    /// </summary>
    public void MoveCardToDeck(Card card)
    {
        card.transform.SetParent(transform);
        card.transform.SetAsFirstSibling();
	}

    public Card GetTopCard()
    {
        if (transform.childCount == 0) return null;

        return transform.GetChild(0).GetComponent<Card>();
    }

	public void AddNewCardToDeck(Placement placementToSpawn, bool isRevealed)
	{
		GameObject newCardObject = new GameObject();
		newCardObject.transform.parent = transform;
		Card newCard = newCardObject.AddComponent<Card>();

		newCard.PlacementToSpawn = placementToSpawn;
		newCard.IsRevealed = isRevealed;
        newCard.gameObject.name = placementToSpawn.gameObject.name;
        newCard.transform.SetParent(transform);
	}
}
