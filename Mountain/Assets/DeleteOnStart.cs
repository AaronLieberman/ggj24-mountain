using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnStart : MonoBehaviour
{
    private void Awake()
    {
        GameObject.DestroyImmediate(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
