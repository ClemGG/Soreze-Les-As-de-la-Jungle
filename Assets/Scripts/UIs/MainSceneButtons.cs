using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneButtons : MonoBehaviour
{


    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]


    [Tooltip("Les différents boutons du canvas qui seront réactivés à la fin d'une épreuve.")]
    public GameObject[] panelButtons;

    [Tooltip("Les différents boutons du canvas qui seront désactivés pendant une épreuve.")]
    public GameObject[] panelButtonsToHideOnEpreuve;

    [Space(20)]

    public GameObject[] panels;





    [Space(10)]
    [Header("Dialogue : ")]
    [Space(10)]

    public GameObject panelDiscussion;
    public GameObject panelEpreuve;
    public GameObject panelPhoto;
    public GameObject panelQuit;
    public GameObject contentPanelBag;

    bool panelEpreuveWasActive = false;





    [Space(10)]
    [Header("Help : ")]
    [Space(10)]


    int currentLevel = -1;  //Utilisée pour savoir si l'on doit réinitialiser le sac ou non





    [Space(10)]
    [Header("Cinematique intro : ")]
    [Space(10)]

    [Tooltip("Variable exposée bloquant l'utilisation des boutons tant que l'intro se joue.")]
    public bool introDone = false;


    public static MainSceneButtons instance;

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



    //Appelée par les boutons de la scène ppale et par les panels du canvas pour afficher un panel précis
    public void TogglePanel(GameObject panel)
    {


        //On ne change pas de panel si l'intro du jeu ou des épreuves se joue
        //ou si l'on fait un fondu en noir vers la scène suivante

        if (panel.name != panelEpreuve.name && panelEpreuve.activeSelf || 
            !introDone && panel != panelDiscussion && panel != panelEpreuve || 
            ScreenTransitionImageEffect.instance.isTransitioning && introDone ||
            !introDone && panel == contentPanelBag)
        {
            return;
        }

        //On désactive le panel en cours et on affiche le nouveau
        if (!panel.activeSelf)
        {
            panel.SetActive(true);

            for (int i = 0; i < panels.Length; i++)
            {
                if(panels[i].name != panel.name)
                {
                    DisablePanel(panels[i]);
                }

            }
        }
        else
        {
            DisablePanel(panel);
        }
    }


    private void DisablePanel(GameObject panel)
    {
        Animator a = panel.GetComponent<Animator>();

        if (a)
        {
            a.Play("hide");
        }
        else
        {
            panel.SetActive(false);

        }
    }


    //Pour affiche soit le panel des dialgues de discussion (en plein écran dans la scène ppale)
    //soit le panel des dialogues des épreuves (en bas de l'écran)
    public void ToggleDialoguePanel(DialogueTrigger.DialogueType dialogueType)
    {
        if(dialogueType == DialogueTrigger.DialogueType.Discussion)
        {
            panelDiscussion.SetActive(true);
            panelEpreuve.SetActive(false);
        }
        else
        {
            panelDiscussion.SetActive(false);
            panelEpreuve.SetActive(true);
        }
    }



    //Appelée par le bouton Home pour retourner au menu ppal
    public void ReturnToMainMenu()
    {
        //On ferme tous les panels ouverts
        TogglePanel(panels[0]);
        int level = SceneManager.GetActiveScene().buildIndex;

        //Si on est dans la scène photo ou celle des moustiques, on supprime le dialogue de victoire
        //pour ne pas avoir le panel de discussion ouvert
        if (level >= 7) 
        {
            GameObject go = FindObjectOfType<MainSceneDialogueList>().gameObject;
            if(go)
                Destroy(go);
        }
        else if(level != 1)
        {
            //Sinon, on marque l'épreuve comme terminée pour ne pas pouvoir la continuer pendant la transition de retour
            FindObjectOfType<Epreuve>().EpreuveFinished = true;
        }
        
        //On retourne à la scène ppale si on est en cours d'épreuve, au menu ppal sinon
        ScreenTransitionImageEffect.instance.FadeToScene(level == 1 ? 0 : 1);
    }




    private void OnLevelWasLoaded(int level)
    {
        currentLevel = level;

        //Si on est de retour au menu ppal, on reset le sac des oeuvres et on détruit ce gameObject
        if(level == 0)
        {
            contentPanelBag.transform.parent.GetComponent<BagPanelButtons>().ResetAllOeuvresAndCollectibles();
            contentPanelBag.transform.parent.GetComponent<BagPanelButtons>().OnEnable();

            Destroy(gameObject);
            return;
        }
        else
        {
            //Si on est de retour à la scène ppale, on réinitialise tous les scripts des panels
            if(level == 1)
            {
                for (int i = 0; i < panelButtons.Length; i++)
                {
                    panelButtons[i].SetActive(true);
                }

            }
            else
            {
                for (int i = 0; i < panelButtonsToHideOnEpreuve.Length; i++)
                {
                    panelButtonsToHideOnEpreuve[i].SetActive(false);
                }
            }

        }
    }

    //Quand on quitte le jeu depuis la scène ppale, on vide les PlayerPrefs pour effacer la progression des joueurs ainsi que les paramètres (son et langue)
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
}
