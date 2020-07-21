using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedImg : MonoBehaviour
{

    public Sprite imgFR, imgEN, imgES;


    public void ChangeImg()
    {
        //On  récupère le component Image et on change l'image en fonction de la langue
        Image img = GetComponent<Image>();
        string language = LocalizationManager.instance.currentLanguage;

        if (img) img.sprite = language == "fr" ? imgFR : language == "en" ? imgEN : imgES;
    }
}
