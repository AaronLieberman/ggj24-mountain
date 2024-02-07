using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacementAction : MonoBehaviour
{
    [Tooltip("Just a comment field to explain why this action is on a Placement.")]
    public string Comment;
    public abstract void DoWork(Worker worker, Placement placement, Card card);
}
