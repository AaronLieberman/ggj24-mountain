using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShowOnVisitPopupTextAction : PlacementAction
{
    public string textToPopUp = "A ShowPopUpText had no text set.";
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        Debug.Log(textToPopUp);

        Utilities.GetRootComponent<GameManager>().ShowOnRevealText(placement.Name, textToPopUp);
    }
}
