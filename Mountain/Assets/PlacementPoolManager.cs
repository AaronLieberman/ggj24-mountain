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

	public void AddToDeckFromCurrentPool(int numOfCardsToAdd = 1, bool isRevealed = false, bool addAtTopOfDeck = false)
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

			_playerDeck.AddNewCardToDeck(randomPlacement, isRevealed, addAtTopOfDeck);
		}
	}

	public void AddToDeckFromBiomeInPool(Placement MatchingBiome, int numOfCardsToAdd = 1, bool isRevealed = false, bool addAtTopOfDeck = false)
    {
        CardPool currentCardPool = GetCurrentCardPool();

        if (currentCardPool.tilePlacementObjects.Count == 0)
        {
            Debug.Log("Current CardPool is empty. Did you populate it using the import cards script?");
            return;
        }


        List<int> matchingIndices = FindAllCardsOfBiomeInCurrentPool(MatchingBiome, currentCardPool);
        //_playerDeck.AddNewCardToDeck(randomPlacement.GetComponent<Placement>(), isRevealed, addAtTopOfDeck);

        // Hey Aaron, I wanted to have a lambda that just fills in this one value for the AddNewCardToDeck() function,
        // But lambdas want a return value, and can't be void. So I, uuuh... worked around that.
        // This feels kinda gross. Is there a better way to do this?
        Func<Placement, bool, bool> actOnCard = (Placement p, bool i) => {_playerDeck.AddNewCardToDeck(p, i, addAtTopOfDeck); return true;};

        SelectRandomCards(matchingIndices, numOfCardsToAdd, isRevealed, actOnCard, currentCardPool);
    }

    private static void SelectRandomCards(List<int> matchingIndices, int numOfCardsToAdd, bool isRevealed, Func<Placement, bool, bool> ActOnCard, CardPool currentCardPool)
    {

        System.Random random = new System.Random();
        for (int i = 0; i < numOfCardsToAdd; i++)
        {
            // Check if there are matching cards
            if (matchingIndices.Count > 0)
            {
                int randomIndex = random.Next(0, matchingIndices.Count);

                // Retrieve the random card from the current pool
                GameObject randomPlacementObject = currentCardPool.tilePlacementObjects[matchingIndices[randomIndex]];
                Placement randomPlacement = randomPlacementObject.GetComponent<Placement>();
                ActOnCard(randomPlacement.GetComponent<Placement>(), isRevealed);

                // Add the random card to the player's deck
            }
            else
            {
                Debug.Log("No matching cards found in the current pool.");
            }
        }
    }

    private static List<int> FindAllCardsOfBiomeInCurrentPool(Placement MatchingBiome, CardPool currentCardPool)
    {
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

        return matchingIndices;
    }

    public void AddToDeckFromAllBiomesInPool(int numberOfFields = 0, int numberOfWoods = 0, int numberOfSettlements = 0, int numberOfSwamps = 0, int numberOfWastelands = 0, int numberOfRandom = 0, bool isRevealed = false, bool addAtTopOfDeck = false)
	{
		Deck deck = Utilities.GetRootComponent<Deck>();
		AddToDeckFromBiomeInPool(deck.FieldsBiome, numberOfFields, isRevealed, addAtTopOfDeck);
		AddToDeckFromBiomeInPool(deck.WoodsBiome, numberOfWoods, isRevealed, addAtTopOfDeck);
		AddToDeckFromBiomeInPool(deck.SettlementBiome, numberOfSettlements, isRevealed, addAtTopOfDeck);
		AddToDeckFromBiomeInPool(deck.SwampBiome, numberOfSwamps, isRevealed, addAtTopOfDeck);
		AddToDeckFromBiomeInPool(deck.WastelandBiome, numberOfWastelands, isRevealed, addAtTopOfDeck);

		AddToDeckFromCurrentPool(numberOfRandom, isRevealed, addAtTopOfDeck);
    }

    //
	public Placement GetRandomCardFromBiome(Placement biome)
	{
        Placement randomPlacement = null;
        List<int> matchingIndices = FindAllCardsOfBiomeInCurrentPool(biome, GetCurrentCardPool());

        SelectRandomCards(matchingIndices, 1, false, (Placement p, bool i) => randomPlacement = p, GetCurrentCardPool());

        return randomPlacement;
    }

    public CardPool GetCurrentCardPool()
	{
		return cardPools[currentCardPoolIndex];
	}
}
