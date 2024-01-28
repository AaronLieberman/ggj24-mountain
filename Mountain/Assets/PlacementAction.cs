using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacementAction : MonoBehaviour
{
    public abstract void DoWork(Worker worker, Placement placement, Card card);
}
