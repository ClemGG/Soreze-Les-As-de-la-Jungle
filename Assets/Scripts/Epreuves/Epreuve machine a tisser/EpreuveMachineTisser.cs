using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EpreuveMachineTisser : Epreuve
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    [Tooltip("Les pièces de la machine à replacer")]
    public MachinePiece[] machinePieces;

    [Space(10)]

    [Tooltip("Les emplacements de la machine où replacer les objets")]
    public MachineSlot[] machineSlots;

    [Space(10)]

    public GameObject planPanel, btnPlanPanel, btnReplay;

    [Space(10)]

    [Tooltip("Les boutons de la boîte à outils")]
    public Button[] allButtonsInInventory;

    [Space(10)]

    public CameraControllerMachineTisser cam;
    public DialogueList noticeDialogueList;

    [Space(20)]

    public Image tissuVladimir;

    [Space(10)]
    [Header("Pieces : ")]
    [Space(10)]

    public int currentPieceIndex = 0;
    int piecesFound = 0;

    [Space(10)]

    public float delayBeforeSendingHelp = 180f;
    float helpTimer;

    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip bgmClip;
    public AudioClip goodClip;
    public AudioClip errorClip;
    public AudioClip victoryClip;
    public AudioClip outilsClip;


    public AudioClip[] piecesInOrderClips;



    #region Epreuve

    //Appelé par les boutons de la boîte à outils pour instancier une nouvelle pièce à placer
    public void SpawnPiece(int index)
    {
        TogglePlanPanel();
        MachinePiece mp = machinePieces[index];


        //On récupère l'index de la nouvelle pièce et on bloque les boutons que l'on a déjà replacés
        for (int i = 0; i < machinePieces.Length; i++)
        {

            machinePieces[i].gameObject.SetActive(i == index && !machineSlots[i].done);
            allButtonsInInventory[i].interactable = i != index && !machineSlots[i].done;
            
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
        //Destroy(machinePiece.gameObject);
        machineSlot.done = true;
        machineSlot.ShowBase(true);



        ObjectPooler.instance.SpawnFromPool("poof", machineSlot.transform.position, Quaternion.identity);

        //On active le collider de la nouvelle pièce placée et on va chercher la consigne pour la pièce suivante
        currentPieceIndex += increaseIndex ? 1 : 0;
        EnableCurColider(currentPieceIndex);
        DialogueEpreuveSystem.instance.gameObject.SetActive(true);
        DialogueEpreuveSystem.instance.ReadReplique(GetCorrectNoticeDialogue().repliques[currentPieceIndex]);

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
                if (piecesInOrderClips[currentPieceIndex])
                    AudioManager.instance.Play(piecesInOrderClips[currentPieceIndex]);
                else
                    AudioManager.instance.Play(goodClip);
            }
            else
            {
                AudioManager.instance.Play(errorClip);
            }
        }
    }


    public void CheckVictory()
    {

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
        DialogueEpreuveSystem.instance.gameObject.SetActive(true);
        DialogueEpreuveSystem.instance.ReadReplique(GetCorrectNoticeDialogue().repliques[currentPieceIndex]);
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

            planPanel.SetActive(!planPanel.activeSelf);
            btnPlanPanel.SetActive(!btnPlanPanel.activeSelf);
        }
            AudioManager.instance.Play(outilsClip);
    }


    #endregion



    //Ces 3 fonctions s'occupent d'afficher le tissu de Vladimir au milieu de dialogue de victoire


    public void ShowTissuVladimir()
    {
        StartCoroutine(ShowTissuCo(1f));

        for (int i = 0; i < dialoguesVictory.listeDialogueEpreuve.Count; i++)
        {
            dialoguesVictory.listeDialogueEpreuve[i].repliques[1].onRepliqueStarted -= ShowTissuVladimir;
        }
    }
    public void HideTissuVladimir()
    {
        StartCoroutine(ShowTissuCo(-1f));

        for (int i = 0; i < dialoguesVictory.listeDialogueEpreuve.Count; i++)
        {
            dialoguesVictory.listeDialogueEpreuve[i].repliques[6].onRepliqueEnded -= ShowTissuVladimir;
        }
    }
    private IEnumerator ShowTissuCo(float increaseValue)
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








    #region Overrides



    protected override IEnumerator Start()
    {
        //Dans le Start, on cache la boîte à outils et le bouton replayNotice

        btnPlanPanel.SetActive(false);
        btnReplay.SetActive(false);
        yield return StartCoroutine(base.Start());

        planPanel.SetActive(false);
        btnPlanPanel.SetActive(false);
        btnReplay.SetActive(true);

        for (int i = 0; i < machinePieces.Length; i++)
        {

            MachinePiece mp = machinePieces[i];
            mp.gameObject.SetActive(false);
            yield return null;
        }

        scoreText.text = "0";
        finalScoreText.text = "/" + allButtonsInInventory.Length.ToString();




        tissuVladimir.gameObject.SetActive(false);

        //Pour afficher le bout de tissu de vladimir dans le dialogue de victoire
        for (int i = 0; i < dialoguesVictory.listeDialogueEpreuve.Count; i++)
        {
            dialoguesVictory.listeDialogueEpreuve[i].repliques[1].onRepliqueStarted += ShowTissuVladimir;
            dialoguesVictory.listeDialogueEpreuve[i].repliques[6].onRepliqueEnded += HideTissuVladimir;
        }

        EnableCurColider(0);
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
        //print(index);

        //A la première aide, on affiche ttes le spièces manquantes en transparent
        if(index == 0)
        {
            for (int i = 0; i < machineSlots.Length; i++)
            {
                if(!machineSlots[i].done)
                    machineSlots[i].ShowTransparent();
            }
        }

        //Pour les pièces 2 à 5, on affiche en surbrillance l'endroit où mettre la prochaine pièce
        else 
        {
            if (index == 1 || index == 3)
            {
                machineSlots[currentPieceIndex].ShowSurbrillance();

            }
            else if (index == 2 || index == 4)
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
                    piecesFound = allButtonsInInventory.Length;
                    PlacePieceOnSlot(machinePieces[i], machineSlots[i], true);
                    //On n'augmente pas le currentPieceIndex ici si l'on souhaite afficher le score du joueur / le nombre de pièces qu'il a manquées
                }

                OnEpreuveEnded(true);
            }
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
