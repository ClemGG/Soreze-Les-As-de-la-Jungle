using System.Collections;
using UnityEngine;

public class EpreuveComparaison : Epreuve
{
    #region Variables

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]


    [SerializeField] GameObject cross;
    [SerializeField] Animator juniorAnim;
    Camera cam;

    [Space(10)]
    [SerializeField] ComparaisonSprite[] motifsToReveal;
    [Space(10)]






    [Space(10)]
    [Header("Raycast : ")]
    [Space(10)]

    [Tooltip("Le layerMask des triggers des sprites à faire apparaître avec le projectile")]
    [SerializeField] LayerMask paintingMask;
    RaycastHit hit;
    Vector3 mousePos;
    public bool isShooting = false;


    [Space(10)]
    [Header("Motifs : ")]
    [Space(10)]

    [SerializeField] DialogueList motifsDialogueList;
    int currentMotifIndex = 0;
    int motifsFound = 0;


    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    [SerializeField] AudioClip goodClip;
    [SerializeField] AudioClip errorClip;
    [SerializeField] AudioClip victoryClip;
    public AudioClip splashClip;
    public AudioClip impactClip;
    public AudioClip posePapierClip;


    #endregion





    #region Epreuve



    //Appelée par l'aide
    public void HighlightDifferences(int index)
    {
        if (EpreuveFinished)
            return;


        cross.transform.position = mousePos;


        if (index == -1 || index != currentMotifIndex)
        {
            cross.SetActive(true);
            ReplayNotice();
            AudioManager.instance.Play(errorClip);
        }
        else
        {
            if (index == currentMotifIndex)
            {
                cross.SetActive(false); 


                motifsToReveal[index].RevealSprite();
                motifsFound++;
                UpdateScoreUI();


            }
        }

    }



    //Vérifie si les conditions de victoire sont réunies
    public void CheckVictory(int index)
    {
        //Si on a tout rempli, on joue le dialogue de victoire
        if (motifsFound == motifsToReveal.Length)
        {
            OnEpreuveEnded(true);
            AudioManager.instance.Play(victoryClip);
        }
        else
        {
            //Sinon, on lance le dialogue du motif suivant
            DisplayRandomReplique();
            AudioManager.instance.Play(goodClip);

        }

        ObjectPooler.instance.SpawnFromPool("success", motifsToReveal[index].transform.position, Quaternion.identity);
    }



    public void ShootProjectile()
    {
        if (EpreuveFinished)
            return;


        //Si on touche la toile, on lance l'animation de tir, qui se chargera du reste
        if (Physics.Raycast(cam.ViewportPointToRay(Vector2.one / 2f), out hit, 100f, paintingMask))
        {
            if (juniorAnim) juniorAnim.Play("a_junior_jump");
        }
    }


    //Appelée par le GameEvent de l'anim pour vérifier si on a bien touché la cible
    public void CheckHitAfterAnim()
    {

        //Si on touche la toile...
        if (Physics.Raycast(cam.ViewportPointToRay(Vector2.one / 2f), out hit, 100f, paintingMask))
        {
            mousePos = hit.point;




            //Si on touche un des sprites, on le révèle
            if (hit.collider.CompareTag("comparaison/painting"))
            {

                if(hit.collider.TryGetComponent(out ComparaisonSprite cs))
                {
                    HighlightDifferences(cs.ID == currentMotifIndex ? currentMotifIndex : -1);

                    if (cs.ID == currentMotifIndex)
                        CheckVictory(currentMotifIndex);
                }
                
            }
            else
            {
                //Sinon, on affiche une erreur
                HighlightDifferences(-1);
            }


        }





        Animator splashAnim = ObjectPooler.instance.SpawnFromPool("splash junior", mousePos, Quaternion.identity).GetComponent<Animator>();
        if(splashAnim) splashAnim.Play("wall junior");

        AudioManager.instance.Play(impactClip);
        AudioManager.instance.Play(posePapierClip);


    }










    public override void UpdateScoreUI()
    {
        scoreText.text = motifsFound.ToString();
    }







    //Affiche la consigne liée au sprite en cours
    //Si on veut rendre le choix des sprites aléatoire, le faire ici
    private void DisplayRandomReplique()
    {
        

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


    //Rejoue la consigne si le joueur a râté son tir
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

#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
            ShootProjectile();
#endif
    }


    //Affiche le bouton du caméléon
    public override void SendHelp()
    {
        if (EpreuveFinished)
            return;

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
        ResetHelpTimer();

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
