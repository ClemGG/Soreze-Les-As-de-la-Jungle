using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class ScanPanelButtons : MonoBehaviour
{
    [Tooltip("Les différents composants de ce panel.")]
    public GameObject panelScan, panelErreur, panelAnalyse, panelLoading, panelValidation;

    [Tooltip("Le temps de recherche avant d'afficher le message indiquant qu'aucune oeuvre n'a été trouvée.")]
    public float delayBeforeError = 10f;

    float timer;

    public bool isScanning = false;
    ARTrackedImage curTrackedImage;

    [Tooltip("Permet de passer outre la vérification des épreuves pour accéder à la photo finale.")]
    public bool bypass = false;

    [Tooltip("Le dialogue joué quand l'épreuve photo est toujours verrouillée.")]
    public DialogueList unlockPhotoDialogList;

    [Tooltip("Le dialogue joué quand le scan réussi tpour la première fois.")]
    public DialogueList unlockScanDialogList;

    [Space(20)]

    [Tooltip("Permet de ne jouer le dialogue de scan qu'une seule fois.")]
    bool scanDialogueDone = false;

    [Space(20)]
    [Header("Audio : ")]
    [Space(20)]

    public AudioClip startScanClip;
    public AudioClip scanLoopClip;
    public AudioClip scanCompleteClip;
    public AudioClip scanErrorClip;

    public static ScanPanelButtons instance;


    //Singleton
    private void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }


    //Quand le panel de scan est activé, on réinitialise le timer et les sous-panels
    public void OnEnable()
    {
        timer = 0f;
        isScanning = true;

        ShowHidePanel(panelScan, true);
        ShowHidePanel(panelAnalyse, false);
        ShowHidePanel(panelLoading, false);
        ShowHidePanel(panelValidation, false);
        ShowHidePanel(panelErreur, false);

        AudioManager.instance.Play(startScanClip);
        AudioManager.instance.Play(scanLoopClip);
    }


    //Quand le panel de scan est désactivé, on réinitialise le timer
    public void OnDisable()
    {
        AudioManager.instance.Play(startScanClip);
        AudioManager.instance.Stop(scanLoopClip);
        isScanning = false;
        timer = 0f;
    }


    void Update()
    {
        ////pour les tests
        //if (Input.GetMouseButtonDown(0) && !panelErreur.activeSelf)
        //{
        //    if (!shouldCount)
        //    {
        //        shouldCount = true;
        //        ShowHidePanel(panelScan, false);
        //        ShowHidePanel(panelAnalyse, true);
        //        ShowHidePanel(panelLoading, true);
        //        ShowHidePanel(panelValidation, false);
        //    }
        //    else
        //    {
        //        timer = 0f;
        //        shouldCount = false;
        //        ShowHidePanel(panelLoading, false);
        //        ShowHidePanel(panelValidation, true);
        //    }
        //}

        //On continue de scanner tant que le timer n'est pas atteint, puis on affiche le panel d'erreur
        if (isScanning)
        {
            if (timer < delayBeforeError)
            {
                timer += Time.deltaTime;
            }
            else
            {
                if (ScreenTransitionImageEffect.instance.isTransitioning)
                    return;

                isScanning = false;
                ShowHidePanel(panelScan, false);
                ShowHidePanel(panelErreur, true);

                AudioManager.instance.Stop(scanLoopClip);
                AudioManager.instance.Play(scanErrorClip);

                timer = 0f;
            }
        }
    }

    public void ShowHidePanel(GameObject panel, bool show)
    {

        Animator a = panel.GetComponent<Animator>();

        if (show)
        {
            if (a)
            {
                a.Play("show");
            }
            else
            {
                panel.SetActive(true);
            }
        }
        else
        {
            if (a)
            {
                a.Play("hide");
            }
            else
            {
                panel.SetActive(false);
            }
        }


        
    }


    //Tout est dans le nom
    public void ShowDialogueFirstScanThenLoadEpreuve(ARTrackedImage trackedImage)
    {
        curTrackedImage = trackedImage;
        StartCoroutine(ShowDialogueFirstScanThenLoadEpreuveCo());
    }



    private IEnumerator ShowDialogueFirstScanThenLoadEpreuveCo()
    {
        //Avant d'afficher le premier dialogue, on réinitialise tous les paramètres et on lance la fenêtre de validation
        //indiquant au joueur qu'il a réussi

        int levelToLoad = ScreenTransitionImageEffect.GetSceneIndexByName(curTrackedImage.referenceImage.name);
        isScanning = false;
        timer = 0f;

        ShowHidePanel(panelAnalyse, true);
        ShowHidePanel(panelScan, false);
        ShowHidePanel(panelLoading, true);
        ShowHidePanel(panelValidation, false);
        AudioManager.instance.Play(scanLoopClip);

        yield return new WaitForSeconds(2f);

        ShowHidePanel(panelLoading, false);
        ShowHidePanel(panelValidation, true);
        AudioManager.instance.Stop(scanLoopClip);
        AudioManager.instance.Play(scanCompleteClip);

        yield return new WaitForSeconds(2f);

        ShowHidePanel(panelValidation, false);
        ShowHidePanel(panelAnalyse, false);




        //Une fois que c'est fait, on lance le dialogue de scan réussi et on lance l'épreuve, 
        //et si on passe la vérif de l'épreuve photo, on peut lancer l'épreuve de photo aussi

        if (!bypass)
        {
            //Si on ne bypasse pas, on vérifie que les épreuves ont ttes été faites
            bool allDone = true;
            if (levelToLoad == 9)
            {

                for (int i = 0; i < 6; i++)
                {

                    if (PlayerPrefs.GetInt($"EpreuveVictory{i}") == 0)
                    {
                        allDone = false;
                        break;
                    }
                }

            }

            //Si oui, on lance le dialogue du premier scan si besoin et ensuite l'épreuve

            if (allDone)
            {
                if (!scanDialogueDone)
                {
                    scanDialogueDone = true;
                    DialogueTrigger d = FindObjectOfType<DialogueTrigger>();
                    d.dialogueType = DialogueTrigger.DialogueType.Epreuve;
                    DialogueEpreuveSystem.instance.onDialogueEnded += LoadFirstScene;


                    d.PlayNewDialogue(unlockScanDialogList);
                }
                else
                {
                    ScreenTransitionImageEffect.instance.FadeToScene(levelToLoad);
                }
            }

            //Sinon, on affiche le dialogue d'épreuve photo verrouillée

            else
            {
                DialogueTrigger d = FindObjectOfType<DialogueTrigger>();
                d.dialogueType = DialogueTrigger.DialogueType.Epreuve;
                d.PlayNewDialogue(unlockPhotoDialogList);
            }
        }
        else
        {

            //La même chose qu'au dessus mais sans la vérif puisqu'on la passe

            if (!scanDialogueDone)
            {
                scanDialogueDone = true;
                DialogueTrigger d = FindObjectOfType<DialogueTrigger>();
                d.dialogueType = DialogueTrigger.DialogueType.Epreuve;
                DialogueEpreuveSystem.instance.onDialogueEnded += LoadFirstScene;


                d.PlayNewDialogue(unlockScanDialogList);
            }
            else
            {
                ScreenTransitionImageEffect.instance.FadeToScene(levelToLoad);
            }

        }


        


        yield return null;
    }

    //On lance la scène portant le même nom de l'image traquée par le scan
    private void LoadFirstScene()
    {
        ScreenTransitionImageEffect.instance.FadeToScene(ScreenTransitionImageEffect.GetSceneIndexByName(curTrackedImage.referenceImage.name));
        DialogueEpreuveSystem.instance.onDialogueEnded -= LoadFirstScene;

    }

    //public void ResetTimer()
    //{
    //    timer = 0f;
    //    shouldCount = true;
    //    DialogueEpreuveSystem.instance.onDialogueEnded -= ResetTimer;
    //}
}
