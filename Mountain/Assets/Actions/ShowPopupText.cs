using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShowPopupText : PlacementAction
{
    public string textToPopUp = "A ShowPopUpText had no text set.";
    public override void DoWork(Worker worker, Placement placement, Card card)
    {
        //TODO: show popup dialog instead of going to the log
        Debug.Log(textToPopUp);
    }
}
