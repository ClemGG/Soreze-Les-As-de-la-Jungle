using UnityEngine;


//Nous permet de cacher les items du sac une fois que l'on retourne au menu ppal
// (si l'on a terminé le jeu ou cliqué sur le bouton retour menu)

public class ResetBagOnStartup : MonoBehaviour
{
    int previousLevel = 0;

    //Singleton
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }



    private void OnLevelWasLoaded(int level)
    {
        //Quand on arrive sur la scène ppale, on regarde si on vient du menu ppal.
        //Si oui, on cache les items du sac

        if(previousLevel == 0 && level != 0)
        {
            BagPanelButtons b = FindObjectOfType<BagPanelButtons>();
            if(b) b.ResetAllOeuvresAndCollectibles();
        }
        previousLevel = level;

        if(level > 0)
        Destroy(gameObject);
    }
}
