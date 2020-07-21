using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButtons : MonoBehaviour
{
    public Sprite onImg, offImg;
    public string key;

    // Start is called before the first frame update
    void Start()
    {
        ChangeIcon();
    }

    public void ChangeIcon()
    {
        if (PlayerPrefs.HasKey("langue"))
        {
            GetComponent<Image>().sprite = PlayerPrefs.GetString("langue") == key ? onImg : offImg;
        }
        else
        {
            GetComponent<Image>().sprite = key == "fr" ? onImg : offImg;
        }
    }
}
