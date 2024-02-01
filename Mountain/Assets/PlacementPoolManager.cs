using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;
using System;

public class PlacementPoolManager : MonoBehaviour
{
	private static PlacementPoolManager _instance;
	private CardPool[] cardPools;
	[Tooltip("Don't set higher than 3. There are only 4 elements in cardPools.Length")]
	[SerializeField] int currentCardPoolIndex = 0;
	[SerializeField] private Deck playerDeck;

	public static PlacementPoolManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<PlacementPoolManager>();

				if (_instance == null)
				{
					GameObject singletonObject = new GameObject("CardPoolManager");
					_instance = singletonObject.AddComponent<PlacementPoolManager>();
				}
			}

			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}

		CardPool cardPool1 = AssetDatabase.LoadAssetAtPath<CardPool>("Assets/CardPool/CardPool1.asset");
		CardPool cardPool2 = AssetDatabase.LoadAssetAtPath<CardPool>("Assets/CardPool/CardPool2.asset");
		CardPool cardPool3 = AssetDatabase.LoadAssetAtPath<CardPool>("Assets/CardPool/CardPool3.asset");
		CardPool cardPool4 = AssetDatabase.LoadAssetAtPath<CardPool>("Assets/CardPool/CardPool4.asset");

		cardPools = new CardPool[] { cardPool1, cardPool2, cardPool3, cardPool4 };
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

	public void AddToDeckFromCurrentPool(int numOfCardsToAdd = 1)
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

			playerDeck.AddNewCardToDeck(randomPlacement, false);
		}
	}

	public void AddToDeckFromBiomeInPool(Placement MatchingBiome, int numOfCardsToAdd = 1)
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
				playerDeck.AddNewCardToDeck(randomPlacement.GetComponent<Placement>(), false);
			}
			else
			{
				Debug.Log("No matching cards found in the current pool.");
			}
		}
	}

	public void AddToDeckFromAllBiomesInPool(int numberOfFields = 0, int numberOfWoods = 0, int numberOfSettlements = 0, int numberOfSwamps = 0, int numberOfWastelands = 0, int numberOfRandom = 0)
	{
		Deck deck = Utilities.GetRootComponent<Deck>();
		AddToDeckFromBiomeInPool(deck.FieldsBiome, numberOfFields);
		AddToDeckFromBiomeInPool(deck.WoodsBiome, numberOfWoods);
		AddToDeckFromBiomeInPool(deck.SettlementBiome, numberOfSettlements);
		AddToDeckFromBiomeInPool(deck.SwampBiome, numberOfSwamps);
		AddToDeckFromBiomeInPool(deck.WastelandBiome, numberOfWastelands);

		AddToDeckFromCurrentPool(numberOfRandom);
    }

	public CardPool GetCurrentCardPool()
	{
		return cardPools[currentCardPoolIndex];
	}
}
