using Clement.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Ce script gère les deux phases de l'épreuve à la fois


public class EpreuveKungfu : Epreuve
{

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]


    public Transform enemiesParent;

    [Space(10)]
    [Header("Epreuve : ")]
    [Space(10)]

    [Tooltip("Les échafaudages à détruire (en phase 2 seulement).")]
    public WeakPoint[] weakPoints;

    [Tooltip("Indique si on est en phase 1 ou 2 de l'épreuve.")]
    public bool isFirstLevel = false;

    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip bgmClip;
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








    #region Epreuve

    //Appelé quand le niveau est rechargé
    public void LooseLife()
    {
        EpreuveKungfuStatic.lives--;

        //print(EpreuveKungfuStatic.lives);



        ScreenTransitionImageEffect.instance.onMiddleOfSceneTransition -= LooseLife;
        //SceneFader.instance.onMiddleOfSceneTransition -= LooseLife;

    }



    //Vérifie si les conditions de victoire sont réunies
    public void CheckVictory()
    {

        if (isFirstLevel)
        {
            UpdateScoreUI();

            if(EpreuveKungfuStatic.nbMoustiquesInScene == 0)
            {
                ObjectPooler.instance.SpawnFromPool("success", Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity);
                AudioManager.instance.Play(victoryClip);
                OnEpreuveEnded(true);
            }


        }
        else
        {

            //Echafaudage[] echafaudages = FindObjectsOfType<Echafaudage>();
            Echafaudage[] echafaudages = SceneManaging.SearchObjectsInSceneOfTypeIncludingDisabled<Echafaudage>().ToArray();

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
                ObjectPooler.instance.SpawnFromPool("success", Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity);
                AudioManager.instance.Play(victoryClip);
                OnEpreuveEnded(true);
            }
        }
    }





    #endregion













    #region Overrides

    //On setup juste le score ici
    protected override IEnumerator Start()
    {
        if (isFirstLevel)
            finalScoreText.text = "/" + ObjectPooler.instance.Pools[1].size.ToString();
        else
            finalScoreText.text = "/" + FindObjectsOfType<Echafaudage>().Length;

        yield return StartCoroutine(base.Start());
        HelpPanelButtons.instance.btnCameleon.gameObject.SetActive(EpreuveKungfuStatic.lives <= 0);


        EpreuveKungfuStatic.nbFourmisInScene = EpreuveKungfuStatic.nbMoustiquesInScene = 0;


        yield return null;

    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }


        base.Update();

    }

    public override void UpdateScoreUI()
    {
        scoreText.text = (ObjectPooler.instance.Pools[1].size - EpreuveKungfuStatic.nbMoustiquesInScene).ToString();

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
            length = Random.Range(2, 4); 
            
        }
        //Au delà, on supprime tout le monde
        else
        {
            length = ms.Length;
        }


        for (int i = 0; i < length; i++)
        {
            ms[i].DestroyThisEnemy();
            ObjectPooler.instance.SpawnFromPool("poof", ms[i].transform.position, Quaternion.identity);
        }
        CheckVictory();

    }






    protected override void OnVictory()
    {

        if (isFirstLevel)
        {
            if (!dialogGoodAnswerGiven && dialogGoodAnswer.listeDialogueEpreuve.Count != 0)
            {
                EpreuveFinished = true;
                DialogueEpreuveSystem.instance.onDialogueEnded += OnEpreuveStarted;
                dialogueTrigger.PlayNewDialogue(dialogGoodAnswer);
                dialogGoodAnswerGiven = true;
            }

            //SceneFader.instance.FadeToScene(SceneFader.CurrentLevelIndex() + 1);
            ScreenTransitionImageEffect.instance.FadeToScene(SceneFader.CurrentLevelIndex() + 1);
        }
        else
        {

            base.OnVictory();

            Exit(false);

        }

        DialogueEpreuveSystem.instance.onDialogueEnded -= OnVictory;

    }

    protected override void OnDefeat()
    {

        if (isFirstLevel)
        {
            //SceneFader.instance.onMiddleOfSceneTransition += LooseLife;
            ScreenTransitionImageEffect.instance.onMiddleOfSceneTransition += LooseLife;
            Exit(true);
        }
        else
        {
            //SceneFader.instance.onMiddleOfSceneTransition += LooseLife;
            ScreenTransitionImageEffect.instance.onMiddleOfSceneTransition += LooseLife;
            //SceneFader.instance.FadeToScene(SceneFader.CurrentLevelIndex() - 1);
            ScreenTransitionImageEffect.instance.FadeToScene(ScreenTransitionImageEffect.CurrentLevelIndex() - 1);
        }

        DialogueEpreuveSystem.instance.onDialogueEnded -= OnDefeat;
    }

    #endregion
}
