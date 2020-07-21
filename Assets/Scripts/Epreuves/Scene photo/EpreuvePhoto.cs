using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpreuvePhoto : Epreuve
{


    [Space(10)]
    [Header("Dialogue Epreuve photo : ")]
    [Space(10)]

    public MainSceneDialogueList mainSceneDialogueList;
    public DialogueList dialogueGrandIntro, dialogueBasIntro, dialogueRecommencer, dialogueMotif;
    [HideInInspector] public bool dialogueMotifHasPlayed = false;

    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip bgmClip;
    public AudioClip goodClip;
    public AudioClip errorClip;
    public AudioClip victoryClip;

    public AudioClip photoClip;
    public AudioClip startPhotoClip;
    public AudioClip loadingPhotoClip;
    public AudioClip donePhotoClip;




    // Start is called before the first frame update
    protected override IEnumerator Start()
    {
        MainSceneButtons.instance.TogglePanel(MainSceneButtons.instance.panelPhoto);


        EpreuveFinished = true;

        string str = LocalizationManager.instance.currentLanguage;

        if (epreuveInfos.Length > 0)
        {
            int epreuveInfoIndex = str == "fr" ? 0 : str == "en" ? 1 : 2;
            currentEpreuveInfo = epreuveInfos[epreuveInfoIndex];
        }
        HelpPanelButtons.instance.btnCameleon.gameObject.SetActive(false);



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




        if (panelIntro)
        {
            titreIntroText.text = currentEpreuveInfo.nomEpreuve;

            panelIntro.SetActive(true);
            yield return new WaitForSecondsRealtime(5f);
            panelIntro.SetActive(false);
        }

        dialogueTrigger.dialogueType = DialogueTrigger.DialogueType.Discussion;
        DialogueDiscussionSystem.instance.onDialogueEnded += PlayEpreuveDialogue;
        dialogueTrigger.PlayNewDialogue(dialogueGrandIntro);


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
        }
    }


    public void ReturnToMainMenu()
    {
        ScreenTransitionImageEffect.instance.FadeToScene(0);

        foreach (Dialogue d in mainSceneDialogueList.newDialogueDiscussionList.listeDialogueEpreuve)
        {
            d.repliques[d.repliques.Length - 1].onRepliqueEnded -= ReturnToMainMenu;
        }
    }

}
