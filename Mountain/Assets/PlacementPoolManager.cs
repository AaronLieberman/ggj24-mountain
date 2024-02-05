using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;
using System;

public class PlacementPoolManager : MonoBehaviour
{
	public CardPool[] cardPools;
	[Tooltip("Don't set higher than 3. There are only 4 elements in cardPools.Length")]
	[SerializeField] int currentCardPoolIndex = 0;

	Deck _playerDeck;

	private void Awake()
	{
		_playerDeck = Utilities.GetRootComponent<Deck>();
	}

	/// <returns>new CardPool Index</returns>
	public int IncrementCardPool()
	{
		if (currentCardPoolIndex >= cardPools.Length - 1)
		{
			Debug.Log("Already at max Card Pool!");
			return currentCardPoolIndex;
		}

		currentCardPoolIndex++;

		return currentCardPoolIndex;
	}

	public void AddToDeckFromCurrentPool(int numOfCardsToAdd = 1, bool isRevealed = false)
	{
		System.Random random = new System.Random();
		CardPool currentCardPool = GetCurrentCardPool();

		for (int i = 0; i < numOfCardsToAdd; i++)
		{
			int randomIndex = random.Next(0, currentCardPool.tilePlacementObjects.Count);

			GameObject randomPlacementObject = currentCardPool.tilePlacementObjects[randomIndex];
			Placement randomPlacement = randomPlacementObject.GetComponent<Placement>();
			if (randomPlacement == null)
			{
				Debug.LogError("Object doesn't have a placement script on it!");
				return;
			}

			_playerDeck.AddNewCardToDeck(randomPlacement, isRevealed);
		}
	}

	public void AddToDeckFromBiomeInPool(Placement MatchingBiome, int numOfCardsToAdd = 1, bool isRevealed = false)
	{
		CardPool currentCardPool = GetCurrentCardPool();

		if (currentCardPool.tilePlacementObjects.Count == 0)
		{
			Debug.Log("Current CardPool is empty. Did you populate it using the import cards script?");
			return;
		}

		System.Random random = new System.Random();

		// Create a list to store indices of cards that match the condition
		List<int> matchingIndices = new List<int>();

		// Find all indices of cards in the current pool that match the condition
		for (int i = 0; i < currentCardPool.tilePlacementObjects.Count; i++)
		{
			GameObject cardGameObject = currentCardPool.tilePlacementObjects[i];
			Placement placement = cardGameObject.GetComponent<Placement>();

			if (placement == null)
			{
				Debug.Log("No placement found!");
				continue;
			}
			else if (placement.Biome == null)
			{
				Debug.Log("placement.Biome is null");
				continue;
			}

			if (placement.Biome == MatchingBiome)
			{
				matchingIndices.Add(i);
			}
		}

		for (int i = 0; i < numOfCardsToAdd; i++)
		{
			// Check if there are matching cards
			if (matchingIndices.Count > 0)
			{
				int randomIndex = random.Next(0, matchingIndices.Count);

				// Retrieve the random card from the current pool
				GameObject randomPlacementObject = currentCardPool.tilePlacementObjects[matchingIndices[randomIndex]];
				Placement randomPlacement = randomPlacementObject.GetComponent<Placement>();

				// Add the random card to the player's deck
				_playerDeck.AddNewCardToDeck(randomPlacement.GetComponent<Placement>(), isRevealed);
			}
			else
			{
				Debug.Log("No matching cards found in the current pool.");
			}
		}
	}

	public void AddToDeckFromAllBiomesInPool(int numberOfFields = 0, int numberOfWoods = 0, int numberOfSettlements = 0, int numberOfSwamps = 0, int numberOfWastelands = 0, int numberOfRandom = 0, bool isRevealed = false)
	{
		Deck deck = Utilities.GetRootComponent<Deck>();
		AddToDeckFromBiomeInPool(deck.FieldsBiome, numberOfFields, isRevealed);
		AddToDeckFromBiomeInPool(deck.WoodsBiome, numberOfWoods, isRevealed);
		AddToDeckFromBiomeInPool(deck.SettlementBiome, numberOfSettlements, isRevealed);
		AddToDeckFromBiomeInPool(deck.SwampBiome, numberOfSwamps, isRevealed);
		AddToDeckFromBiomeInPool(deck.WastelandBiome, numberOfWastelands, isRevealed);

		AddToDeckFromCurrentPool(numberOfRandom, isRevealed);
    }

	public CardPool GetCurrentCardPool()
	{
		return cardPools[currentCardPoolIndex];
	}
}
