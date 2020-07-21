using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComparaisonSprite : MonoBehaviour
{


    [Space(10)]
    [Header("Colours : ")]
    [Space(10)]


    //public string ID;
    [HideInInspector] public int ID;
    [HideInInspector] public bool done = false;
    public Material defaultMat;
    [HideInInspector] public SpriteRenderer sr;

    private EpreuveComparaison epreuve;

    private void Start()
    {
        epreuve = (EpreuveComparaison)Epreuve.instance;
        sr = GetComponent<SpriteRenderer>();
        done = false;
        ID = transform.GetSiblingIndex() - 1;
    }

    public void RevealSprite()
    {
        done = true;
        sr.material = defaultMat;
    }


}
