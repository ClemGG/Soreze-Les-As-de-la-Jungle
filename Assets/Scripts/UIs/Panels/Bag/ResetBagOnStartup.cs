using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Nous permet de cacher les items du sac une fois que l'on retourne au menu ppal
// (si l'on a terminé le jeu ou cliqué sur le bouton retour menu)

public class ResetBagOnStartup : MonoBehaviour
{
    int previousLevel = 0;

    public static ResetBagOnStartup instance;

    //Singleton
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }



    private void OnLevelWasLoaded(int level)
    {
        //Quand on arrive sur la scène ppale, on regarde si on vient du menu ppal.
        //Si oui, on cache les items du sac

        if(previousLevel == 0)
        {
            BagPanelButtons b = FindObjectOfType<BagPanelButtons>();
            if(b) b.ResetAllOeuvresAndCollectibles();
        }
        previousLevel = level;
    }
}
