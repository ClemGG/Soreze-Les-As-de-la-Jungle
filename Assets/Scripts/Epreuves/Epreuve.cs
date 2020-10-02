using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Epreuve : MonoBehaviour
{
    #region Variables

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]


    [Tooltip("Le DialogueTrigger utilisé pour afficher les dialogues d'intro et de fin d'épreuve")]
    [SerializeField] protected DialogueTrigger dialogueTrigger;

    [Space(10)]

    [Tooltip("Les dialogues qui apparaissent quand le joueur fait une bonne ou mauvais action.")]
    public DialogueList dialogGoodAnswer, dialogBadAnswer;

    protected bool dialogGoodAnswerGiven = false, dialogBadAnswerGiven = false;





    [Space(10)]
    [Header("Epreuve : ")]
    [Space(10)]

    [Tooltip("Variable exposée qui permet de bloquer les actions du joueur pendant les dialogues et les instructions.")]
    public bool EpreuveFinished = true;

    [Space(10)]

    [Tooltip("Le nb d'aides autorisées par épreuve.")]
    public int nbHelp;

    public float delayBeforeHelp = 60f;
    protected float _helpTimer;
    protected int currentHelpIndex = 0;

    [Space(10)]

    [SerializeField] protected TextMeshProUGUI scoreText;
    [SerializeField] protected TextMeshProUGUI finalScoreText;


    [Space(10)]

    [SerializeField] protected DialogueList dialoguesVictory, dialoguesDefeat;

    [Space(10)]

    [Tooltip("Utilisé pour les PlayerPrefs et le déblocage des oeuvres.")]
    [SerializeField] protected int epreuveID;






    [Space(10)]
    [Header("Epreuve Instructions : ")]
    [Space(10)]

    [Tooltip("Les instructions de l'épreuve.")]
    [SerializeField] protected LocalizedEpreuve[] epreuveInfos;
    protected LocalizedEpreuve currentEpreuveInfo;
    [Space(10)]

    [SerializeField] protected GameObject panelInstructions;
    [SerializeField] protected GameObject panelIntro;
    [SerializeField] protected TextMeshProUGUI titreIntroText;
    [SerializeField] protected TextMeshProUGUI titreEpreuveText;
    [SerializeField] protected TextMeshProUGUI instructionsText;




    public static Epreuve instance;


    #endregion



    #region Mono





    //Singleton
    protected void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;


    }



    protected virtual IEnumerator Start()
    {
        //Quand on démarre l'épreuve, on bloque les actions du joueur
        EpreuveFinished = true;





        //On récfupère les infos traduites pour les afficher juste avant le début de l'épreuve
        string str = LocalizationManager.instance.currentLanguage;
        if (epreuveInfos.Length > 0)
        {
            int epreuveInfoIndex = str == "fr" ? 0 : str == "en" ? 1 : 2;
            currentEpreuveInfo = epreuveInfos[epreuveInfoIndex];
        }




        HelpPanelButtons.instance.cameleon.gameObject.SetActive(false);




        if (panelInstructions)
        {
            titreEpreuveText.text = currentEpreuveInfo.nomEpreuve;
            instructionsText.text = currentEpreuveInfo.instructions;

            panelInstructions.SetActive(false);
            DialogueEpreuveSystem.instance.onDialogueEnded += ShowPanelInstructions;
        }
        else
        {
            DialogueEpreuveSystem.instance.onDialogueEnded += OnEpreuveStarted;
        }


        //On affiche d'abord le dialogue d'intro de l'épreuve, puis ses instructions avant de lancer le défi
        if (panelIntro)
        {
            titreIntroText.text = currentEpreuveInfo.nomEpreuve;

            panelIntro.SetActive(true);

            WaitForSeconds wait = new WaitForSeconds(5f);
            yield return wait;

            panelIntro.SetActive(false);
        }


        dialogueTrigger.PlayNewDialogue();

        //if(session) session.enabled = true;

    }

    protected virtual void Update()
    {

        if (EpreuveFinished)
            return;

        if (_helpTimer < delayBeforeHelp)
        {
            _helpTimer += Time.deltaTime;
        }
        else
        {
            HelpPanelButtons.instance.ShowHelp();
        }
    }



    #endregion




    //Une fois l'aide utilisée, on relance le compteur
    public void ResetHelpTimer()
    {
        _helpTimer = 0f;
    }

    //Quand le joueur appelle le caméléon manuellement, annuler le compteur pour l'afficher
    public void SetHelpTimerToZero()
    {
        _helpTimer = delayBeforeHelp;
    }

    protected virtual void PlayGoodBadDialogue(bool good)
    {

    }

    public virtual void UpdateScoreUI()
    {

    }








    protected virtual void ShowPanelInstructions()
    {
        panelInstructions.SetActive(true);
    }

    public virtual void HidePanelInstructions()
    {
        OnEpreuveStarted();
    }








    //Cette fonction permet d'initialiser les données de l'épreuve en elle-même quand elle commence
    protected virtual void OnEpreuveStarted()
    {


        if (panelInstructions)
            panelInstructions.SetActive(false);

        EpreuveFinished = false;
        DialogueEpreuveSystem.instance.onDialogueEnded -= OnEpreuveStarted;
        DialogueEpreuveSystem.instance.onDialogueEnded -= ShowPanelInstructions;


        //On libère de la place en mémoire.
        //On l'appelle une fois l'épreuve démarée car c'est l'endroit le moins sensible niveau
        //baisse des performances
        ApplicationManager.CollectGarbage();


    }

    //Quand l'épreuve est terminée, on affiche le dialogue de victoire et on bloque les actions du joueur
    protected virtual void OnEpreuveEnded(bool victory)
    {
        //On libère de la place en mémoire.
        //On l'appelle une fois l'épreuve démarée car c'est l'endroit le moins sensible niveau
        //baisse des performances
        ApplicationManager.CollectGarbage();

        ShowDialogue(victory);
    }






    //Quand on gagne l'épreuve, on indique au BagPanel que l'oeuvre est déblouquée et on enregistre la progression du joueur
    protected virtual void OnVictory()
    {
        BagPanelButtons.instance.ChangeOeuvreLockState(epreuveID);
        MainSceneButtons.instance.TogglePanel(DialogueEpreuveSystem.instance.dialoguePanel);

        PlayerPrefs.SetInt($"EpreuveVictory{epreuveID}", 1);



        //On a réussi une épreuve, on indique au jeu qu'une partie a été sauvegardée
        //pour réactiver le bouton Reprendre dans le menu ppal.
        //Le contenu n'a pas d'importance, on vérifie juste si la clé existe
        PlayerPrefs.SetInt("partie en cours", 0);
    }

    //Quand on perd l'épreuve, soit le joeur la recommence, soit il est renvoyé à la scène ppale
    protected virtual void OnDefeat()
    {

    }



    //Quand le timer d'aide est terminé, on active le boutyon du caméléon pour donner une indice au joueur
    public virtual void SendHelp()
    {
        if (EpreuveFinished)
            return;

        GiveSolutionToPlayer(currentHelpIndex);

        if (currentHelpIndex < nbHelp)
        {
            currentHelpIndex++;
            ResetHelpTimer();
        }
    }

    //Affiche des indices / résout une partie de l'épreuve à la place du joueur en fonction du nombre d'aides qu'il a déjà demandées
    public virtual void GiveSolutionToPlayer(int index)
    {

    }



    protected void ShowDialogue(bool victory)
    {
        EpreuveFinished = true;
        PlayerPrefs.SetInt($"EpreuveVictory{epreuveID}", victory ? 1 : 0);

        dialogueTrigger.PlayNewDialogue(victory ? dialoguesVictory : dialoguesDefeat);


        if (victory)
        {
            DialogueEpreuveSystem.instance.onDialogueEnded += OnVictory;
        }
        else
        {
            DialogueEpreuveSystem.instance.onDialogueEnded += OnDefeat;
        }
    }

    //On quitte l'épreuve
    protected void Exit(bool restart)
    {
        //On désactive la RA pour le niveau vu qu'on a fini pour gagner des ressources
        //if(session) session.enabled = false;

        //SceneFader.instance.FadeToScene(restart ? SceneManager.GetActiveScene().buildIndex : 1);
        ScreenTransitionImageEffect.instance.FadeToScene(restart ? SceneManager.GetActiveScene().buildIndex : 1);
        DialogueEpreuveSystem.instance.onDialogueEnded -= OnDefeat;
        DialogueEpreuveSystem.instance.onDialogueEnded -= OnVictory;
        DialogueEpreuveSystem.instance.onDialogueEnded -= OnEpreuveStarted;
    }

}
