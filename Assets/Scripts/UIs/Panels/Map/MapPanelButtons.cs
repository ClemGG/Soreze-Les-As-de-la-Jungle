using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapPanelButtons : MonoBehaviour
{
    public Location[] locations;
    public Sprite normalImg, checkImg;

    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip openMapClip;


    public void OnEnable()
    {

        AudioManager.instance.Play(openMapClip);

        //Quand le panel de la map s'active, on récupère tous les points indiquant la position d'une oeuvre
        //et on regarde si l'épreuve correspondante a été terminée.
        //Si oui, on affiche le symbole Valider

        for (int i = 0; i < locations.Length; i++)
        {
            Location l = locations[i];

            if (PlayerPrefs.HasKey($"EpreuveVictory{l.ID}"))
            {
                l.unlocked = PlayerPrefs.GetInt($"EpreuveVictory{l.ID}", 0) == 1 ? true : false;
                l.img.sprite = l.unlocked ? checkImg : normalImg;
            }
            else
            {
                l.unlocked = false;
                l.img.sprite = normalImg;
            }
        }
    }

}
