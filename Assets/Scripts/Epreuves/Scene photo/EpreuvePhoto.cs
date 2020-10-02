using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class EpreuvePhoto : Epreuve
{

    #region Variables

    [Space(10)]
    [Header("Dialogue Epreuve photo : ")]
    [Space(10)]

    public ARSession session;

    [SerializeField] Canvas canvasEpreuve;
    [SerializeField] MainSceneDialogueList mainSceneDialogueList;
    [SerializeField] DialogueList dialogueGrandIntro, dialogueBasIntro, dialogueRecommencer, dialogueMotif;

    [Space(20)]

    public GameObject characters;

    [HideInInspector] public bool dialogueMotifHasPlayed = false;

    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip photoClip;
    public AudioClip startPhotoClip;
    public AudioClip loadingPhotoClip;
    public AudioClip donePhotoClip;



    #endregion


    #region Overrides

    // Start is called before the first frame update
    protected override IEnumerator Start()
    {

        session = FindObjectOfType<ARSession>();

        //Une fois l'épreuve commencée, on réactive l'occlusion
        EnableArOcclusion(true);


        EpreuveFinished = true;

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



        //Pour l'AR, on rapproche le canvas pour que l'occlusion ne le bloque pas
        canvasEpreuve.planeDistance = .3f;




        if (panelIntro)
        {
            titreIntroText.text = currentEpreuveInfo.nomEpreuve;

            panelIntro.SetActive(true);
            yield return new WaitForSecondsRealtime(5f);
            panelIntro.SetActive(false);
        }

        MainSceneButtons.instance.TogglePanel(MainSceneButtons.instance.panelPhoto);


        //EDIT : Finalement on ne joue plus le dialogue en grand écran, seulement le petit

        //dialogueTrigger.dialogueType = DialogueTrigger.DialogueType.Discussion;
        //DialogueDiscussionSystem.instance.onDialogueEnded += PlayEpreuveDialogue;
        //dialogueTrigger.PlayNewDialogue(dialogueGrandIntro);


        dialogueTrigger.dialogueType = DialogueTrigger.DialogueType.Epreuve;
        dialogueTrigger.PlayNewDialogue(dialogueBasIntro);


        //Pour l'AR, on éloigne le canvas (mis à 1 par défaut dans ScreenTransition)
        canvasEpreuve.planeDistance = 5f;

    }


    protected override void OnVictory()
    {
        //L'épreuve de photo n'a pas d'oeuvre associée, donc on ne la débloque pas dans la sacoche
        MainSceneButtons.instance.TogglePanel(DialogueEpreuveSystem.instance.dialoguePanel);
        PlayerPrefs.SetInt($"EpreuveVictory{epreuveID}", 1);


    }

    #endregion




    #region Epreuve

    //Appelée dans la delgate de PanelPhotoButtons au moment de la capture d'écran
    //afin de cacher les personnages pour n'afficher que le fond
    //(on s'en moque de l'index, c'est pour que le script ne se plaigne pas)
    //On désactive aussi l'occlusion au même moment
    public void HideCharacters(int index)
    {
        characters.SetActive(false);
    }

    public void EnableArOcclusion(bool active)
    {
        session.enabled = active;
    }




    public void PlayEpreuveDialogue()
    {
        StartCoroutine(WaitForEpreuveDialogue(dialogueBasIntro, 1.5f));
        DialogueDiscussionSystem.instance.onDialogueEnded -= PlayEpreuveDialogue;

    }


    public void PlayMotifDialogue()
    {
        if (!dialogueMotifHasPlayed)
        {
            dialogueMotifHasPlayed = true;
            StartCoroutine(WaitForEpreuveDialogue(dialogueMotif, 1f));

        }

    }

    public void PlayRecommencerDialogue()
    {
        StartCoroutine(WaitForEpreuveDialogue(dialogueRecommencer, 0f));
    }

    private IEnumerator WaitForEpreuveDialogue(DialogueList dialogueList, float delay)
    {
        yield return new WaitForSeconds(delay);

        dialogueTrigger.dialogueType = DialogueTrigger.DialogueType.Epreuve;
        dialogueTrigger.PlayNewDialogue(dialogueList);

    }


    public void EndEpreuve()
    {
        MainSceneButtons.instance.TogglePanel(DialogueEpreuveSystem.instance.dialoguePanel);
        OnVictory();
        foreach (Dialogue d in mainSceneDialogueList.newDialogueDiscussionList.listeDialogueEpreuve)
        {
            d.repliques[d.repliques.Length - 1].onRepliqueEnded += ReturnToMainMenu;
            //d.repliques[d.repliques.Length - 1].onRepliqueEnded += ApplicationManager.OnAppStarted;
        }
    }


    public void ReturnToMainMenu()
    {
        ScreenTransitionImageEffect.instance.FadeToScene(0);

        foreach (Dialogue d in mainSceneDialogueList.newDialogueDiscussionList.listeDialogueEpreuve)
        {
            d.repliques[d.repliques.Length - 1].onRepliqueEnded -= ReturnToMainMenu;
            //d.repliques[d.repliques.Length - 1].onRepliqueEnded -= ApplicationManager.OnAppStarted;
        }
    }



    #endregion
}
