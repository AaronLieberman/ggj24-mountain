using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroUI : MonoBehaviour
{
    [SerializeField] private GameObject FadeContent;

    void FadeComplete()
    {
        FadeContent.SetActive(false);
    }

    void TextComplete()
    {
        gameObject.SetActive(false);
    }
}
