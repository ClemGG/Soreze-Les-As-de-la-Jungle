using System.Collections;
using UnityEngine;

public class LocalizationStartupManager : MonoBehaviour {


    [HideInInspector] public LocalizedText[] textsToModify; //Les Text Components de la scène actuelle
    [HideInInspector] public LocalizedImg[] imagesToModify; //Les Img Components de la scène actuelle

    public static LocalizationStartupManager instance;


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





    /* Quand on arrive dans une nouvelle scène, on appelle la fonction Start qui va chercher tous les Composants Img et Textes
     * de la scène à traduire.
     */

    private void OnLevelWasLoaded(int level)
    {
        StartCoroutine(Start());
    }




    public IEnumerator Start () {

        //On attend une frame pour laisser les composants s'initialiser si besoin
        yield return new WaitForEndOfFrame();

        //On récupère tous les Components traducteurs de la scène
        textsToModify = (LocalizedText[])Resources.FindObjectsOfTypeAll(typeof(LocalizedText));
        imagesToModify = (LocalizedImg[])Resources.FindObjectsOfTypeAll(typeof(LocalizedImg));

        //On attend que le LocalizationManager ait fini de s'initialiser
        //(C'est lui qui est responsable de la traduction en elle-même)
        while (!LocalizationManager.instance.CheckIfReady())
        {
            yield return null;
        }

        //Ensuite, on traduit les items récupérés
        ChangeAllItemsInScene();
    }


    //Appelée dans Start() pour traduire les Compoents Texte et Img de la scène
    public void ChangeAllItemsInScene()
    {
        for (int i = 0; i < textsToModify.Length; i++)
        {
            textsToModify[i].ChangeText();
        }

        for (int i = 0; i < imagesToModify.Length; i++)
        {
            imagesToModify[i].ChangeImg();
        }
    }


}
