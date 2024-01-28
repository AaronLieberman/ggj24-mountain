using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectableCard : MonoBehaviour 
{
    public void OnCardSelected() 
	{
		Utilities.GetRootComponents<Canvas>()
			.Select(c => c.GetComponentInChildren<HandUI>())
			.First().SetSelectedCardUI(GetComponent<CardUI>());
	}
}
