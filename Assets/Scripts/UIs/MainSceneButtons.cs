using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneButtons : MonoBehaviour
{
    #region Variables

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
    [Header("Cinematique intro : ")]
    [Space(10)]

    [Tooltip("Variable exposée bloquant l'utilisation des boutons tant que l'intro se joue.")]
    public bool introDone = false;


    public static MainSceneButtons instance;


    #endregion


    #region Mono

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
        //Pour libérer de la place en mémoire
        ApplicationManager.CollectGarbage();



        if (level == 0)
        {
            Destroy(gameObject);
            return;
        }
        else
        {


            //Si on est de retour à la scène ppale, on réinitialise tous les scripts des panels
            if (level == 1)
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


    #endregion



    #region Menu

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

        //Si on n'a pas réussi l'épreuve, on supprime le dialogue de victoire
        //pour ne pas avoir le panel de discussion ouvert au retour sur la scène ppale
        
        MainSceneDialogueList go = FindObjectOfType<MainSceneDialogueList>();
        if (go)
            Destroy(go.gameObject);
            
        if(level != 1)
        {
            //Sinon, on marque l'épreuve comme terminée pour ne pas pouvoir la continuer pendant la transition de retour
            FindObjectOfType<Epreuve>().EpreuveFinished = true;

        }
        
        //On retourne à la scène ppale si on est en cours d'épreuve, au menu ppal sinon
        ScreenTransitionImageEffect.instance.FadeToScene(level == 1 ? 0 : 1);
    }



    #endregion
}
