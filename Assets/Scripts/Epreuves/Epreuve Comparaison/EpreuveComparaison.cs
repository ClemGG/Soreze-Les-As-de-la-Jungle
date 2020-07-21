using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class EpreuveComparaison : Epreuve
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]



    [Space(10)]
    public ComparaisonSprite[] motifsToReveal;
    [Space(10)]


    [SerializeField] Animator splashAnim;

    [SerializeField] GameObject cross;

    [SerializeField] GameObject correctParticle;

    [SerializeField] GameObject victoryParticle;






    [Space(10)]
    [Header("Raycast : ")]
    [Space(10)]

    [Tooltip("Le layerMask des triggers des sprites à faire apparaître avec le projectile")]
    public LayerMask paintingMask;
    RaycastHit hit;
    public Transform spawnPoint;
    public Camera cam;
    Vector3 mousePos;
    [HideInInspector] public bool isShooting = false;


    [Space(10)]
    [Header("Motifs : ")]
    [Space(10)]

    int motifsFound = 0;
    public DialogueList motifsDialogueList;
    int currentMotifIndex = 0;


    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip bgmClip;
    public AudioClip goodClip;
    public AudioClip errorClip;
    public AudioClip victoryClip;
    public AudioClip splashClip;
    public AudioClip impactClip;
    public AudioClip posePapierClip;


    #region Epreuve



    //Appelée par l'aide
    public void HighlightDifferences(int index)
    {
        if (EpreuveFinished)
            return;


        cross.transform.position = mousePos;


        if (index == -1)
        {
            cross.SetActive(true);
            ResetHelpTimer();
            ReplayNotice();

            AudioManager.instance.Play(errorClip);
        }
        else
        {
            if (index == currentMotifIndex)
            {
                cross.SetActive(false); 
                ResetHelpTimer();


                //motifsToReveal[index].RevealSprite();
                motifsToReveal[index].sr.enabled = true;
                motifsToReveal[index].done = true;

                motifsFound++;
                UpdateScoreUI();


            }
            else
            {
                cross.SetActive(true); 
                ResetHelpTimer();

                ReplayNotice();
                AudioManager.instance.Play(errorClip);

            }
        }

    }

    //Vérifie si les conditions de victoire sont réunies
    public void CheckVictory(int index)
    {

        if (motifsFound == motifsToReveal.Length)
        {
            OnEpreuveEnded(true);
            victoryParticle.SetActive(true);
            victoryParticle.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            AudioManager.instance.Play(victoryClip);
        }
        else
        {
            DisplayRandomReplique();
            correctParticle.SetActive(true);
            correctParticle.transform.position = motifsToReveal[index].transform.position;
            AudioManager.instance.Play(goodClip);

        }
    }



    public void ShootProjectile()
    {
        if (EpreuveFinished || isShooting)
            return;


        if (Physics.Raycast(cam.ViewportPointToRay(Vector2.one / 2f), out hit, 100f, paintingMask))
        {
            isShooting = true;
            ProjectileJunior p = ObjectPooler.instance.SpawnFromPool("projectile junior", spawnPoint.position, Quaternion.identity).GetComponent<ProjectileJunior>();
            mousePos = hit.point;
            p.SetProjectile(currentMotifIndex, mousePos);
            splashAnim.Play("splash");
            AudioManager.instance.Play(splashClip);
        }
    }

    private Vector3 GetMousePosOnPainting()
    {
        float z = cross.transform.position.z;
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(cross.transform.position).z));
        mousePos.z = z;

        return mousePos;
    }

    public override void UpdateScoreUI()
    {
        scoreText.text = motifsFound.ToString();
    }





    //Affiche la consigne liée au sprite en cours
    private void DisplayRandomReplique()
    {
        //List<int> unsolvedQuestions = new List<int>();
        //for (int i = 0; i < motifsToReveal.Length; i++)
        //{
        //    if (!motifsToReveal[i].done)
        //        unsolvedQuestions.Add(i);
        //}

        //int alea = UnityEngine.Random.Range(0, unsolvedQuestions.Count);

        //currentMotifIndex = unsolvedQuestions[alea];

        for (int i = 0; i < motifsToReveal.Length; i++)
        {
            if (!motifsToReveal[i].done)
            {
                currentMotifIndex = i;
                break;
            }
        }


        ReplayNotice();
    }



    private Dialogue GetCorrectNoticeDialogue()
    {
        Dialogue dialogueToPlay = null;
        switch (PlayerPrefs.GetString("langue"))
        {
            case "fr":
                dialogueToPlay = motifsDialogueList.listeDialogueEpreuve[0];
                break;

            case "en":
                dialogueToPlay = motifsDialogueList.listeDialogueEpreuve[1];
                break;

            case "es":
                dialogueToPlay = motifsDialogueList.listeDialogueEpreuve[2];
                break;

            default:
                print("Erreur dialogue : Aucune langue n'a été sélectionnée");
                break;

        }

        return dialogueToPlay;
    }


    //Rejoue la consigne si le joueru a râté son tir
    public void ReplayNotice()
    {
        DialogueEpreuveSystem.instance.gameObject.SetActive(true);
        DialogueEpreuveSystem.instance.ReadReplique(GetCorrectNoticeDialogue().repliques[currentMotifIndex]);
    }





    #endregion



    #region Overrides



    protected override IEnumerator Start()
    {
        scoreText.text = "0";
        finalScoreText.text = $"/{motifsToReveal.Length}";
        cam = Camera.main;
        cross.SetActive(false);

        yield return StartCoroutine(base.Start());
        GameObject.Find("UI proto canvas").GetComponent<Canvas>().sortingLayerName = "Foreground";
    }




    protected override void Update()
    {
        base.Update();

//#if UNITY_EDITOR || UNITY_STANDALONE
//        if (Input.GetMouseButtonDown(0))
//        {
//            ShootProjectile();
//        }
//#endif
    }


    //Affiche le bouton du caméléon
    public override void SendHelp()
    {
        if (EpreuveFinished)
            return;

        HelpPanelButtons.instance.btnCameleon.gameObject.SetActive(false);
        GiveSolutionToPlayer(currentHelpIndex);

        if (currentHelpIndex < nbHelp)
        {
            currentHelpIndex++;
        }
        else
        {
            currentHelpIndex = 0;
        }

    }



    public override void GiveSolutionToPlayer(int index)
    {
        //En dessous de deux indices, on révèle un sprite manquant au joueur.
        //Au delà, on termine le jeu pour lui
        if(index < 2)
        {
            HighlightDifferences(currentMotifIndex);

        }
        else
        {
            for (int i = 0; i < motifsToReveal.Length; i++)
            {
                if (!motifsToReveal[i].done)
                {
                    currentMotifIndex = i;
                    HighlightDifferences(i);
                }
            }

        }
        CheckVictory(currentMotifIndex);

    }

    protected override void OnEpreuveStarted()
    {
        base.OnEpreuveStarted();

        DisplayRandomReplique();

    }






    protected override void OnVictory()
    {
        GameObject.Find("UI proto canvas").GetComponent<Canvas>().sortingLayerName = "Default";
        base.OnVictory();
        Exit(false);
    }
    protected override void OnDefeat()
    {
        Exit(true);
    }
    #endregion
}
