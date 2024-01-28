using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUICanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        Utilities.GetRootComponent<GameManager>().StopMouseActiveHighlight();
    }
}
