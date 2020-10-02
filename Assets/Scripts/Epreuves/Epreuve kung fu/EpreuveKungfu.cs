using Clement.Utilities;
using System.Collections;
using UnityEngine;

//Ce script gère les deux phases de l'épreuve à la fois


public class EpreuveKungfu : Epreuve
{
    #region Variables

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]


    [SerializeField] Transform enemiesParent;
    Transform camT;



    [Space(10)]
    [Header("Epreuve : ")]
    [Space(10)]


    [Tooltip("Indique si on est en phase 1 ou 2 de l'épreuve.")]
    public bool isFirstLevel = false;


    [Tooltip("Si la cible est hors de vue, indiquer au joueur de se rapprocher.")]
    [SerializeField] DialogueList dialogueRapprocheList;


    //Appartient à la 2è partie de l'épreuve
    Echafaudage[] echafaudages;


    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip goodClip;
    public AudioClip errorClip;
    public AudioClip victoryClip;

    public AudioClip langueStartClip1;
    public AudioClip langueStartClip2;
    public AudioClip langueEndClip1;
    public AudioClip langueEndClip2;
    
    public AudioClip explosionBoisClip;
    public AudioClip bambousClip;
    public AudioClip boisClip;
    public AudioClip frappeClip;


    [Space(10)]
    [Header("Victory : ")]
    [Space(10)]

    [Tooltip("Les animations du sprite de Vladimir à jouer.")]
    [SerializeField] Animator vladAnim;

    [Tooltip("La parent contenant tous le smorceaux à projeter.")]
    [SerializeField] Transform statueParent;
    Rigidbody[] rbs;

    [Tooltip("La cage à afficher une fois la statue détruite.")]
    [SerializeField] GameObject cage, planches;

    [Tooltip("L'intervalle de force appliquée sur les morceaux de la statue.")]
    [SerializeField] Vector2 minMaxPieceForce;

    [Tooltip("La fumée permettant de masquer la destruction de la statue et l'apparition de la cage.")]
    [SerializeField] ParticleSystem smokeParticle;



    #endregion



    #region Epreuve




    //Vérifie si les conditions de victoire sont réunies
    public void CheckVictory()
    {

        if (isFirstLevel)
        {
            UpdateScoreUI();

            if(EpreuveKungfuStatic.nbMoustiquesInScene == 0)
            {
                ObjectPooler.instance.SpawnFromPool("success", camT.position + camT.forward, Quaternion.identity);
                AudioManager.instance.Play(victoryClip);
                vladAnim.Play("a_vlad_peur");

                OnEpreuveEnded(true);
            }


        }
        else
        {


            bool victory = true;
            int count = 0;


            for (int i = 0; i < echafaudages.Length; i++)
            {
                if (!echafaudages[i].isDead)
                {
                    victory = false;
                }
                else
                {
                    count++;
                }
            }

            scoreText.text = count.ToString();

            if (victory)
            {
                DestroyStatue();

                ObjectPooler.instance.SpawnFromPool("success", Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity);
                AudioManager.instance.Play(victoryClip);
                vladAnim.Play("a_vlad_chute");
                OnEpreuveEnded(true);
            }
        }
    }



    //Quand on a gagné, on détruit la statue et on enferme Vladimir dans la cage
    private void DestroyStatue()
    {

        StartCoroutine(DestroyStatueCo());
    }

    private IEnumerator DestroyStatueCo()
    {
        smokeParticle.gameObject.SetActive(true);
        planches.gameObject.SetActive(false);
        smokeParticle.Play(true);


        for (int i = 0; i < rbs.Length; i++)
        {
            float alea = Random.Range(minMaxPieceForce.x, minMaxPieceForce.y);
            rbs[i].isKinematic = false;
            rbs[i].AddForce(Vector3.one * alea, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(.75f);

        cage.SetActive(true);

        //Une fois les morceaux projetés, on éteint les Rbs pour économiser
        for(int i = 0; i < rbs.Length; i++)
        {
            
            rbs[i].Sleep();
        }

        yield return null;
    }





    #endregion













    #region Overrides



    //On setup juste le score ici
    protected override IEnumerator Start()
    {
        camT = Camera.main.transform;


        if (isFirstLevel)
        {
            finalScoreText.text = "/" + ObjectPooler.instance.Pools[0].size.ToString();
            EpreuveKungfuStatic.nbMoustiquesInScene = 0;

        }
        else
        {
            finalScoreText.text = "/" + FindObjectsOfType<Echafaudage>().Length;
            echafaudages = SceneManaging.FindAllObjectsInSceneOfType<Echafaudage>().ToArray();
            rbs = statueParent.GetComponentsInChildren<Rigidbody>();
        }



        yield return StartCoroutine(base.Start());



        //Pour s'assurer que le texte du dialogue s'affiche bien devant les grenouilles
        GameObject.Find("Canvas epreuve").GetComponent<Canvas>().sortingOrder = 0;

        yield return null;

    }

    protected override void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }
#endif


        base.Update();

    }





    protected override void OnEpreuveStarted()
    {
        base.OnEpreuveStarted();

        if(isFirstLevel)
            vladAnim.Play("a_vlad_rire");


        ShowRapprocheDialogue();
    }


    public override void UpdateScoreUI()
    {
        scoreText.text = (ObjectPooler.instance.Pools[0].size - EpreuveKungfuStatic.nbMoustiquesInScene).ToString();

    }




    public void ShowRapprocheDialogue()
    {
        if(!EpreuveFinished && !statueParent.gameObject.activeInHierarchy)
        dialogueTrigger.PlayNewDialogue(dialogueRapprocheList);
    }




    //Apelle l'aide
    public override void GiveSolutionToPlayer(int index)
    {
        if (!isFirstLevel)
            return;



        int length = 0;
        Moustique[] ms = FindObjectsOfType<Moustique>();

        //En dessous de 3 aides, on supprime 2-3 moustiques
        if (index < 2)
        {
            if (ms.Length >= 3)
                length = Random.Range(2, 4);
            else
                length = ms.Length;
            
        }
        //Au delà, on supprime tout le monde
        else
        {
            length = ms.Length;
        }


        for (int i = 0; i < length; i++)
        {
            if(!ms[i].isCaught)
            ms[i].DestroyThisEnemy();
        }
        CheckVictory();
        ResetHelpTimer();
    }






    protected override void OnVictory()
    {

        if (isFirstLevel)
        {
            EpreuveFinished = true;
            ScreenTransitionImageEffect.instance.FadeToScene(ScreenTransitionImageEffect.CurrentLevelIndex() + 1);
        }
        else
        {
            base.OnVictory();
            Exit(false);
        }
        DialogueEpreuveSystem.instance.onDialogueEnded -= OnVictory;

    }

    #endregion
}
