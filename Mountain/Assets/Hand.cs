using UnityEngine;

public class Hand : MonoBehaviour
{
    public int MaxHandSize;

    public Deck Deck { get; set; }

    public void Reset()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public bool DrawTillFull()
    {
        for (int i = transform.childCount; i < MaxHandSize; i++)
        {
            var card = Deck.GetTopCard();
            if (card == null) return false;

            card.transform.parent = transform;
        }

        return true;
    }
}
