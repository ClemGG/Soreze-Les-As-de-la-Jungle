using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    public SettingsPanelButtons settingsPanel;


    public static MainMenuButtons instance;

    //Singleton
    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }

        instance = this;

        //Quand on lance l'application, on vide les PlayerPrefs pour effacer la progression des joueurs ainsi que les paramètres (son et langue)
        PlayerPrefs.DeleteAll();
    }



    private void Start()
    {
        //On active le panel des paramètres et on change la langue avant de le refermer

        settingsPanel.gameObject.SetActive(true);

        if (PlayerPrefs.HasKey("langue"))
        {
            settingsPanel.ChangerLangue(PlayerPrefs.GetString("langue"));
        }
        else
        {
            settingsPanel.ChangerLangue("fr");
        }


        settingsPanel.gameObject.SetActive(false);
    }


    //Appelée par le bouton Play
    public void PlayButton()
    {
        //On met la langue par défaut sur FR si le joueur ne l'a pas déjà mis

        if (!PlayerPrefs.HasKey("langue"))
            PlayerPrefs.SetString("langue", "fr");


        //On active les sons
        settingsPanel.gameObject.SetActive(true);
        settingsPanel.ToggleAllSounds(true);
        settingsPanel.gameObject.SetActive(false);



        //On lance la scène ppale
        ScreenTransitionImageEffect.instance.FadeToScene(1);
    }





    //Quand on quitte le jeu, on efface la progression du joueur ainsi que les paramètres (son et langue)
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }


    //Quand on retourne au menu ppal, on efface la progression du joueur ainsi que les paramètres (son et langue)
    private void OnLevelWasLoaded(int level)
    {
        PlayerPrefs.DeleteAll();
    }
}
