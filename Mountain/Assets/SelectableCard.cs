using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectableCard : MonoBehaviour 
{
    public void OnCardSelected() 
	{
		Utilities.GetRootComponent<GameUIManager>().HandUI.SetSelectedCardUI(GetComponent<CardUI>());
	}
}
