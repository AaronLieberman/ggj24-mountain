using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public HandUI HandUI { get; private set; }

    void Awake()
    {
        HandUI = GetComponentInChildren<HandUI>();
    }
}
