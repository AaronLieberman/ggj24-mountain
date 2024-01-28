using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: doesn't work all that well so probably don't use this
// add this as a child
public class Glow : MonoBehaviour
{
    public float ScaleFactor = 1.1f;

    void Start()
    {
        foreach (var sr in transform.parent.GetComponentsInChildren<SpriteRenderer>())
        {
            GameObject glowObject = new GameObject(sr.transform.name + "_glow");
            glowObject.transform.SetParent(transform, false);
            glowObject.transform.localPosition = sr.transform.localPosition;
            glowObject.transform.localScale = sr.transform.localScale;
            glowObject.transform.localRotation = sr.transform.localRotation;

            SpriteRenderer glowSpriteRenderer = glowObject.AddComponent<SpriteRenderer>();
            glowSpriteRenderer.sprite = sr.sprite;
            glowSpriteRenderer.sortingLayerName = sr.sortingLayerName;
            glowSpriteRenderer.sortingOrder = sr.sortingOrder + 1;
            glowSpriteRenderer.color = Color.yellow;
        }
    }
}
