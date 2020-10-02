using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class EpreuveNumeroArt : Epreuve
{

    #region Variables


    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    [Tooltip("Le script de rotation de la roulette")]
    [SerializeField] InfiniteRotation infiniteRotationScript;

    [Tooltip("L'Animator du canon.")]
    [SerializeField] Animator canonAnim;

    [Tooltip("Les symboles Valider à afficher qd une couleur est complétée")]
    [SerializeField] Transform validationIcons;

    [Space(10)]
    [Tooltip("La liste de tous les triggers contenant les sprites de couleur")]
    [SerializeField] NumeroArtSprite[] numeroArtSprites;

    [Space(10)]
    [Header("Raycast : ")]
    [Space(10)]

    [Tooltip("Le layermask des sprites de couleur")]
    [SerializeField] LayerMask paintingMask;

    [Tooltip("Le point de spawn du projectile")]
    [SerializeField] Transform spawnPoint;
    RaycastHit hit;
    Camera cam;

    [SerializeField] float fireRate = .2f;
    float _fireRateTimer;


    [Space(10)]
    [Header("Colours : ")]
    [Space(10)]

    [SerializeField] Color currentColor;
    [SerializeField] int currentColorIndex;

    [Tooltip("La liste des couleurs disponibles sur la roulette, rangées dans l'ordre")]
    [SerializeField] Color[] colorPalette;

    [Tooltip("La liste de tous les sprites de couleur")]
    SpriteNAS[] spritesNAS;
    SpriteNAS[] matchingColors;
    int score;

    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip goodClip;
    public AudioClip errorClip;
    public AudioClip victoryClip;
    
    
    public AudioClip shootClip;
    public AudioClip rouletteClip;
    public AudioClip[] impactClips;

    bool epreuveDone = false;

    #endregion






    #region Epreuve



    //Appelée quand on change de quart sur la roulette
    public void ChangerCouleur(int index)
    {
        currentColor = colorPalette[index]; 
        currentColorIndex = index;
    }

    //Vérifie si les conditions de victoire sont réunies
    public void CheckVictory()
    {
        bool victory = true;

        
        int alea = UnityEngine.Random.Range(0, impactClips.Length);
        AudioManager.instance.Play(impactClips[alea]);


        if (score < validationIcons.childCount)
            victory = false;


        //L'épreuve n'est terminée que si chacune des couleurs a été entièrement rétablie
        if (victory && !epreuveDone)
        {
            epreuveDone = true;
            ObjectPooler.instance.SpawnFromPool("success", cam.transform.position + cam.transform.forward, Quaternion.identity);
            AudioManager.instance.Play(victoryClip);
            OnEpreuveEnded(victory);

        }
    }






    public void ShootCurrentColor()
    {
        if (EpreuveFinished || _fireRateTimer < fireRate)
            return;

        _fireRateTimer = 0f;
        ChangerCouleur(infiniteRotationScript.GetCurrentSection());
        ShootProjectile();
    }


    private void ShootProjectile()
    {
        //print($"Couleur n°{currentColorIndex}");

        if(Physics.Raycast(cam.ViewportPointToRay(Vector2.one/2f), out hit, 100f, paintingMask))
        {
            //print(hit.collider.name);
            Projectile p = ObjectPooler.instance.SpawnFromPool("projectile", spawnPoint.position, Quaternion.identity).GetComponent<Projectile>();
            p.target = hit.point;
            p.SetProjectileColor(currentColor, currentColorIndex);

            AudioManager.instance.Play(shootClip);
            canonAnim.Play("a_tir_canon");
        }
    }

    //Appelée depuis chaque NumeroArtSprite pour déterminer si une couleur est complètement rétablie
    public void UpdateScoreUI(int ID)
    {
        //On récupère l'ID du sprite et on récupère sa couleur associée

        do
        {
            matchingColors = Array.FindAll(spritesNAS, nas => nas.ID == ID);
        }
        while (matchingColors.Length == 0);
        //print($"Couleur : {colorPalette[alea]} ; matchingColors : {matchingColors.Length}");


        //Si tous les sprites de cette couleur ont été repeints, la couleur est rétablie et on update le score
        bool done = true;
        for (int i = 0; i < matchingColors.Length; i++)
        {
            if (!matchingColors[i].done)
            {
                done = false;
                break;
            }
        }
        if (done)
        {
            validationIcons.GetChild(ID).gameObject.SetActive(true);
            score++;
            scoreText.text = score.ToString();
        }
    }


    #endregion










    #region Overrides


    protected override IEnumerator Start()
    {
        //On setup le score

        scoreText.text = "0";
        finalScoreText.text = $"/{colorPalette.Length}";

        for (int i = 0; i < validationIcons.childCount; i++)
        {
            validationIcons.GetChild(i).gameObject.SetActive(false);
        }



        infiniteRotationScript.enabled = false;
        yield return StartCoroutine(base.Start());
        infiniteRotationScript.enabled = true;


        //On récupère tous les sprites de la scène et on réinitialise le compte de couleurs
        spritesNAS = FindObjectsOfType<SpriteNAS>();

        currentColor = Color.white;
        cam = Camera.main;

    }

    //Pour mettre à jour la cadence de tir
    protected override void Update()
    {
        base.Update();


        if (_fireRateTimer < fireRate)
        {
            _fireRateTimer += Time.deltaTime;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShootCurrentColor();
            }
        }
#endif
    }



    public override void GiveSolutionToPlayer(int index)
    {

        //Pour les 4 premières aides, on prend un couleur au hasard et on colorie tous les sprites de cette couleur
        if (index < 4)
        {
            int alea = 0;
            SpriteNAS[] matchingColors = new SpriteNAS[0];
            while(matchingColors.Length == 0)
            {
                alea = UnityEngine.Random.Range(0, colorPalette.Length);
                matchingColors = Array.FindAll(spritesNAS, nas => nas.ID == alea && !nas.done);
            }
            //print($"Couleur : {colorPalette[alea]} ; matchingColors : {matchingColors.Length}");


            for (int i = 0; i < numeroArtSprites.Length; i++)
            {
                for (int j = 0; j < numeroArtSprites[i].sprites.Length; j++)
                {
                    if(numeroArtSprites[i].sprites[j].colorID == alea)
                    {
                        numeroArtSprites[i].AddColor(colorPalette[alea], alea);
                    }
                }
            }

        }
        //A la 5è aide, on colorie tous les sprites manquants
        else
        {
            foreach (NumeroArtSprite nas in numeroArtSprites)
            {
                for (int i = 0; i < nas.sprites.Length; i++)
                {
                    nas.AddColor(colorPalette[nas.sprites[i].colorID], nas.sprites[i].colorID);
                }
            }
        }

        CheckVictory();
        ResetHelpTimer();
    }


    protected override void OnVictory()
    {
        //On affiche toutes les icônes de validation si jamais elles restent cachées à la victoire
        for (int i = 0; i < validationIcons.childCount; i++)
        {
            validationIcons.GetChild(i).gameObject.SetActive(true);
        }

        base.OnVictory();
        Exit(false);
    }
    protected override void OnDefeat()
    {
        for (int i = 0; i < numeroArtSprites.Length; i++)
        {
            if (!numeroArtSprites[i].done)
            {
                //sprites[i].StartCoroutine(sprites[i].BlinkColor(nbBlinkIterations, blinkSpeed));

                if (!dialogBadAnswerGiven && dialogBadAnswer.listeDialogueEpreuve.Count != 0)
                {
                    EpreuveFinished = true;
                    DialogueEpreuveSystem.instance.onDialogueEnded += OnEpreuveStarted;
                    dialogueTrigger.PlayNewDialogue(dialogBadAnswer);
                    dialogBadAnswerGiven = true;
                }
            }
        }
    }

    #endregion
}
