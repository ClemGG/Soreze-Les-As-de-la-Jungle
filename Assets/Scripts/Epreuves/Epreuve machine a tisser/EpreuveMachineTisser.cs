using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EpreuveMachineTisser : Epreuve
{

    #region Variables

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    
    [Tooltip("Les pièces de la machine à replacer")]
    [SerializeField] MachinePiece[] machinePieces;

    [Space(10)]

    [Tooltip("Les emplacements de la machine où replacer les objets")]
    public MachineSlot[] machineSlots;

    [Space(10)]

    public GameObject planPanel, btnPlanPanel, btnReplay, machineParent;

    [Space(10)]

    [Tooltip("Les boutons de la boîte à outils, rangées dans l'ordre qu'ils apparaissent dans la boîte à outils.")]
    [SerializeField] Button[] allButtonsInInventory;

    [Space(10)]

    [SerializeField] CameraControllerMachineTisser cam;
    [SerializeField] DialogueList noticeDialogueList;

    [Space(20)]

    [SerializeField] Image tissuVladimir;

    [Space(10)]
    [Header("Pieces : ")]
    [Space(10)]

    public int currentPieceIndex = 0;

    //Pour éviter le bug qui ferme la boîte à outils et place la pièce au même moment,
    //ce qui cache le bouton d'ouverture de la boite et empêche de terminer l'épreuve
    [HideInInspector] public bool allowedToPlacePiece = true;

    [Space(10)]

    float waitClickTimer;

    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip goodClip;
    public AudioClip errorClip;
    public AudioClip victoryClip;
    public AudioClip outilsClip;


    [SerializeField] AudioClip[] piecesInOrderClips;



    #endregion






    #region Epreuve

    //Appelé par les boutons de la boîte à outils pour instancier une nouvelle pièce à placer
    public void SpawnPiece(int index)
    {

        //Pour éviter le bug qui ferme la boîte à outils et place la pièce au même moment,
        //ce qui cache le bouton d'ouverture de la boite et empêche de terminer l'épreuve
        //Démarre le délai dans l'Update pour bloquer le clic de la souris
        allowedToPlacePiece = false;



        TogglePlanPanel();
        MachinePiece mp = machinePieces[index];


        //On récupère l'index de la nouvelle pièce et on bloque les boutons que l'on a déjà replacés
        for (int i = 0; i < machinePieces.Length; i++)
        {

            machinePieces[i].gameObject.SetActive(i == index && !machineSlots[i].done);

            bool enable = i != index && !machineSlots[i].done;
            allButtonsInInventory[i].interactable = enable;
            
            
        }

        //On instancie la pièce et on la place près de la caméra
        ObjectPooler.instance.SpawnFromPool("poof", cam.piecePosOnThisCam.position, Quaternion.identity);

        if (cam.currentPiece)
        {
            cam.currentPiece.gameObject.SetActive(false);
        }

        mp.transform.position = cam.piecePosOnThisCam.position;
        mp.transform.eulerAngles = cam.piecePosOnThisCam.eulerAngles;
        mp.transform.parent = cam.piecePosOnThisCam;

        cam.currentPiece = mp;
    }










    //Appelé par l'aide et au moment où l'on dépose une nouvelle pièce sur la machine
    public void PlacePieceOnSlot(MachinePiece machinePiece, MachineSlot machineSlot, bool increaseIndex)
    {

        machinePiece.gameObject.SetActive(false);
        machineSlot.done = true;
        machineSlot.ShowBase(true);

        allButtonsInInventory[currentPieceIndex].interactable = false;
        allButtonsInInventory[currentPieceIndex].transform.GetChild(0).gameObject.SetActive(true);



        ObjectPooler.instance.SpawnFromPool("poof", machineSlot.transform.position, Quaternion.identity);

        //On active le collider de la nouvelle pièce placée et on va chercher la consigne pour la pièce suivante
        currentPieceIndex += increaseIndex ? 1 : 0;
        EnableCurColider(currentPieceIndex);

        if (currentPieceIndex <= machinePieces.Length - 1)
        {
            DialogueEpreuveSystem.instance.gameObject.SetActive(true);
            DialogueEpreuveSystem.instance.ReadReplique(GetCorrectNoticeDialogue().repliques[currentPieceIndex]);
        }

        UpdateScoreUI();


        //Ensuite, on vérifie les conditions de victoire et on quitte l'épreuve si on a terminé

        if (currentPieceIndex == machineSlots.Length)
        {
            ObjectPooler.instance.SpawnFromPool("success", cam.transform.position + cam.transform.forward, Quaternion.identity);
            AudioManager.instance.Play(victoryClip);

            btnPlanPanel.SetActive(false);
            OnEpreuveEnded(true);
        }
        else
        {
            if (increaseIndex)
            {
                AudioManager.instance.Play(goodClip);
            }
            else
            {
                AudioManager.instance.Play(errorClip);
            }
        }
    }



    //Pour récupérer la consigne suivante
    private Dialogue GetCorrectNoticeDialogue()
    {
        Dialogue dialogueToPlay = null;
        switch (PlayerPrefs.GetString("langue"))
        {
            case "fr":
                dialogueToPlay = noticeDialogueList.listeDialogueEpreuve[0];
                break;

            case "en":
                dialogueToPlay = noticeDialogueList.listeDialogueEpreuve[1];
                break;

            case "es":
                dialogueToPlay = noticeDialogueList.listeDialogueEpreuve[2];
                break;

            default:
                print("Erreur dialogue : Aucune langue n'a été sélectionnée");
                break;

        }

        return dialogueToPlay;
    }




    
    //Pour relire la consigne en cours
    public void ReplayNotice()
    {
        if (currentPieceIndex < machinePieces.Length - 1)
        {
            DialogueEpreuveSystem.instance.gameObject.SetActive(true);
            DialogueEpreuveSystem.instance.ReadReplique(GetCorrectNoticeDialogue().repliques[currentPieceIndex]);
        }
    }

    //Si on s'est trompé de pièce, on affiche la réplique correspondante
    public void ShowDialogueBadAnswer()
    {
        if (!dialogBadAnswerGiven && dialogBadAnswer.listeDialogueEpreuve.Count != 0)
        {
            //EpreuveFinished = true;
            DialogueEpreuveSystem.instance.onDialogueEnded += OnEpreuveStarted;
            dialogueTrigger.PlayNewDialogue(dialogBadAnswer);

            dialogBadAnswerGiven = true;
        }
    }



    //Pour afficher / cacher le texte de consigne en bas de l'écran
    public void TogglePlanPanel()
    {
        if (EpreuveFinished)
        {

            planPanel.SetActive(false);
            btnPlanPanel.SetActive(false);
        }
        else
        {


            if (DialogueEpreuveSystem.instance.gameObject.activeSelf)
            {
                DialogueEpreuveSystem.instance.WriteAlltext();
                DialogueEpreuveSystem.instance.NextReplique();


            }

            //Pour éviter le bug qui ferme la boîte à outils et place la pièce au même moment,
            //ce qui cache le bouton d'ouverture de la boite et empêche de terminer l'épreuve
            //Démarre le délai dans l'Update pour bloquer le clic de la souris
            allowedToPlacePiece = false;

            planPanel.SetActive(!planPanel.activeSelf);
            btnPlanPanel.SetActive(!btnPlanPanel.activeSelf);
        }
            AudioManager.instance.Play(outilsClip);
    }




    //Ces 3 fonctions s'occupent d'afficher le tissu de Vladimir au milieu de dialogue de victoire


    public void ShowTissuVladimir()
    {

        for (int i = 0; i < dialoguesVictory.listeDialogueEpreuve.Count; i++)
        {
            dialoguesVictory.listeDialogueEpreuve[i].repliques[1].onRepliqueStarted -= ShowTissuVladimir;
        }

        StartCoroutine(ShowTissuCo(1f));
    }
    public void HideTissuVladimir()
    {

        for (int i = 0; i < dialoguesVictory.listeDialogueEpreuve.Count; i++)
        {
            int length = dialoguesVictory.listeDialogueEpreuve[i].repliques.Length-1;
            dialoguesVictory.listeDialogueEpreuve[i].repliques[length].onRepliqueEnded -= HideTissuVladimir;
        }


        StartCoroutine(ShowTissuCo(-1f));

    }

    public IEnumerator ShowTissuCo(float increaseValue)
    {

        tissuVladimir.gameObject.SetActive(true);

        if (Mathf.Sign(increaseValue) == 1)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                tissuVladimir.color = new Color(tissuVladimir.color.r, tissuVladimir.color.g, tissuVladimir.color.b, t);
                yield return null;
            }
        }
        else
        {
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                tissuVladimir.color = new Color(tissuVladimir.color.r, tissuVladimir.color.g, tissuVladimir.color.b, t);
                yield return null;
            }
        }

        tissuVladimir.gameObject.SetActive(Mathf.Sign(increaseValue) == 1);
    }




    //Pour afficher le collider de la pièce dernièrement réparée
    private void EnableCurColider(int index)
    {
        for (int i = 0; i < machineSlots.Length; i++)
        {
            Collider[] cols = machineSlots[i].gameObject.GetComponentsInChildren<Collider>();
            for (int j = 0; j < cols.Length; j++)
            {
                cols[j].enabled = i == index;
            }
        }
    }



    #endregion














    #region Overrides



    protected override IEnumerator Start()
    {
        //Au démarrage, on affiche les pièces en transparence
        //(on active aussi le parent car il est désactivé par l'ImageTracker)
        machineParent.SetActive(true);
        for (int i = 0; i < machineSlots.Length; i++)
        {
            if (!machineSlots[i].done)
                machineSlots[i].ShowTransparent();
        }



        //Dans le Start, on cache la boîte à outils et le bouton replayNotice

        for (int i = 0; i < machinePieces.Length; i++)
        {
            allButtonsInInventory[i].transform.GetChild(0).gameObject.SetActive(false);


            MachinePiece mp = machinePieces[i];
            mp.gameObject.SetActive(false);
            yield return null;
        }


        scoreText.text = "0";
        finalScoreText.text = "/" + allButtonsInInventory.Length.ToString();


        btnPlanPanel.SetActive(false);
        btnReplay.SetActive(false);
        yield return StartCoroutine(base.Start());

        planPanel.SetActive(false);
        btnPlanPanel.SetActive(false);
        btnReplay.SetActive(true);

       





        //Pour afficher le bout de tissu de vladimir dans le dialogue de victoire
        tissuVladimir.gameObject.SetActive(false);
        for (int i = 0; i < dialoguesVictory.listeDialogueEpreuve.Count; i++)
        {
            int length = dialoguesVictory.listeDialogueEpreuve[i].repliques.Length-1;
            dialoguesVictory.listeDialogueEpreuve[i].repliques[1].onRepliqueStarted += ShowTissuVladimir;
            dialoguesVictory.listeDialogueEpreuve[i].repliques[length].onRepliqueEnded += HideTissuVladimir;
        }

        EnableCurColider(0);
    }

    protected override void Update()
    {
        base.Update();



        //Pour éviter le bug qui ferme la boîte à outils et place la pièce au même moment,
        //ce qui cache le bouton d'ouverture de la boite et empêche de terminer l'épreuve
        //Démarre le délai dans l'Update pour bloquer le clic de la souris

        if (waitClickTimer < .1f && !allowedToPlacePiece)
        {
            waitClickTimer += Time.deltaTime;
        }
        else
        {
            allowedToPlacePiece = true;
            waitClickTimer = 0f;
        }
    }






    public override void UpdateScoreUI()
    {
        scoreText.text = currentPieceIndex.ToString();
    }


    public override void HidePanelInstructions()
    {
        base.HidePanelInstructions();
        DialogueEpreuveSystem.instance.gameObject.SetActive(true);
        DialogueEpreuveSystem.instance.ReadReplique(GetCorrectNoticeDialogue().repliques[currentPieceIndex]);
    }


    //Quand l'épreuve commence
    protected override void OnEpreuveStarted()
    {
        base.OnEpreuveStarted();
        btnPlanPanel.SetActive(true);
        DialogueEpreuveSystem.instance.onDialogueEnded -= OnEpreuveStarted;


    }


    //Apelle l'aide
    public override void GiveSolutionToPlayer(int index)
    {
        ResetHelpTimer();

        if (index == 0 || index == 2)
        {
            machineSlots[currentPieceIndex].ShowSurbrillance();

        }
        else if (index == 1 || index == 3)
        {
            PlacePieceOnSlot(machinePieces[currentPieceIndex], machineSlots[currentPieceIndex], true);
        }


        //Pour la dernière aide, on met ttes les pièces restantes
        else
        {
            for (int i = 0; i < machineSlots.Length; i++)
            {
                if (i < currentPieceIndex)
                    continue;

                //print($"{i} {currentPieceIndex}");
                PlacePieceOnSlot(machinePieces[i], machineSlots[i], true);
                //On n'augmente pas le currentPieceIndex ici si l'on souhaite afficher le score du joueur / le nombre de pièces qu'il a manquées
            }

            OnEpreuveEnded(true);
        }
    }



    protected override void OnVictory()
    {
        base.OnVictory();
        Exit(false);
    }
    protected override void OnDefeat()
    {
        Exit(true);
    }


    #endregion
}
