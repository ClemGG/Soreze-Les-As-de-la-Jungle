using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//utilisé pendant les épreuves pour afficher le bouton du caméléon et les aides.

public class HelpPanelButtons : MonoBehaviour
{
    public static HelpPanelButtons instance;
    public Transform btnCameleon;


    //Singleton
    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
    }


    //Au début de chaque niveau, on cache le caméléon
    private void OnLevelWasLoaded(int level)
    {
        //if(level == 1 && MainSceneButtons.instance.introDone)
        //{
        //    print("true");
        //    btnCameleon.gameObject.SetActive(false);

        //}
        //    print("false");
        if(MainSceneButtons.instance.introDone)
        StartCoroutine(DisableCameleon());
            
    }

    private IEnumerator DisableCameleon()
    {
        yield return new WaitForSeconds(1f);
        btnCameleon.gameObject.SetActive(false);
    }



    //Appelée par le btn du caméléon
    public void GetEpreuveHelp()
    {
        Epreuve e = FindObjectOfType<Epreuve>();

        if (e)
        {
            if (!e.EpreuveFinished)
            {
                e.SendHelp();
            }
        }
    }
}
