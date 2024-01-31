using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroFade : MonoBehaviour
{
    public GameObject Fade;

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        canvas.enabled = true;
    }

    void FadeComplete()
    {
        Fade.SetActive(false);
    }

    void TextComplete()
    {
        gameObject.SetActive(false);
    }
}
