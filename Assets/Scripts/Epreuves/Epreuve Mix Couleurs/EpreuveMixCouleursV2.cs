using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EpreuveMixCouleursV2 : Epreuve
{
    #region Variables

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    [Tooltip("L'animator pour faire bouger le bol")]
    [SerializeField] Animator bolAnim;
    Coroutine bolAnimCo;


    [Tooltip("Le script responsable du mélange des couleurs")]
    public ColorCheckerV3 bolResultat;

    [Tooltip("Les particules à afficher qd un mélange est réalisé")]
    [SerializeField] GameObject correctParticle, successParticle, erreurParticle;

    [Tooltip("Les traces de peinture de la calebasse à afficher une fois une couleur ajoutée")]
    [SerializeField] Image[] tracesPeinture;

    [Space(10)]

    [Tooltip("Le ScriptableObject contenant la liste des combinaisons de couleur à réaliser")]
    [SerializeField] ColorCombinationScriptableObject allCombinationsSO;
    public Button validateBtn, clearButton;

    [Space(10)]

    [SerializeField] Canvas canvasBols;
    [SerializeField] Canvas canvasEpreuve;


    [Space(10)]

    [HideInInspector] public bool isDragging = false;
    [HideInInspector] public bool isAnimating = false;

    [Tooltip("Le point où doivent s'arrêter les bols avant de verser leur couleur")]
    public RectTransform animEndPoint; 

    [Tooltip("Le sprite avant de la calebasse. Utilisé pour afficher correctement les effets de particule")]
    public Canvas calebasseFront;








    [Space(10)]
    [Header("Colors : ")]
    [Space(10)]

    int curNbOfColorsCreated = 0;

    [Space(10)]

    [Tooltip("Les icônes des résultats finaux sur la liste de couleurs en arrière-plan")]
    [SerializeField] ColorUI[] allColorUIs;

    [Space(10)]

    [Tooltip("Variable exposée affichant la combinaison actuellement dans le bol.")]
    [HideInInspector] public List<ColorID> currentCombination;






    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip goodClip;
    public AudioClip errorClip;
    public AudioClip victoryClip;

    public AudioClip touilleClip;
    public AudioClip verseClip;
    public AudioClip videBolClip;







    [Space(10)]
    [Header("Anim d'intro : ")]
    [Space(10)]

    [Tooltip("Les deux bols à déplacer pour remplir la 1ère couleur automatiquement.")]
    [SerializeField] BolDraggable firstAnimBol;
    [SerializeField] BolDraggable secondAnimBol;
    [HideInInspector] public bool stopIntroAnim = false;    //Utilisé pour arrêter les bols une fois l'anim de remplissage lancée


    #endregion


    #region Epreuve avec bols




    //Appelée par les pots de couleur au moment du trigger
    public void AddColorToMix(ColorID id)
    {
        //On arrête d'ajouter des couleurs si la calebasse est pleine
        if (currentCombination.Count == 3)
            return;

        currentCombination.Add(id);

        //Si la calebasse est vide ou est en pleine animation, on désactive le bouton valider
        if (bolResultat.isAnimating || bolResultat.full || currentCombination.Count < 2)
        {
            validateBtn.interactable = false;
        }
        else if (!EpreuveFinished && currentCombination.Count > 1)
        {
            validateBtn.interactable = true;

        }


        //On affiche une nouvelle trace de peinture
        StartCoroutine(ShowTracesPeintureCo(allCombinationsSO.GetColorFromID(id), currentCombination.Count-1));
    }


    //Appelée par le bouton reset
    public void ResetColors() 
    {
        bolAnim.Play("a_mix_error");
        validateBtn.interactable = false;

        currentCombination.Clear();
        bolResultat.ResetFioleColor();

        //On cache les traces de peinture
        StopAllCoroutines();
        StartCoroutine(ShowTracesPeintureCo(Color.clear));
    }



    //Si on veut faire disparaître les traces, on fait un lerp vers Color.clear
    //Sinon, le lerp ira de Color.clear (donc transparent) à la couleur de la peinture
    //S'il n'y a as d'index passé en pramètre, on change toutes les traces
    private IEnumerator ShowTracesPeintureCo(Color newColor, int index = -1)
    {
        
        float timer = 0f;

        while(timer < 1f)
        {
            timer += Time.deltaTime * 1.5f;

            for (int i = 0; i < tracesPeinture.Length; i++)
            {
                if (index == -1 || index == i)
                { 
                    Color c = tracesPeinture[i].color;
                    tracesPeinture[i].color = Color.Lerp(c, newColor, timer);
                }
            }

            yield return null;
        }
    }




    public override void UpdateScoreUI()
    {
        int x = 0;

        for (int i = 0; i < allColorUIs.Length; i++)
        {
            if (allColorUIs[i].done)
            {
                x++;
            }
        }

        scoreText.text = x.ToString();



    }






    //Appelée par le bouton de mélange
    public void ValidateCombination(bool auto)
    {

        bool correct = false;
        int index = -1;
        ColorCombination curCC = null;

        //On compare notre combinaison avec notre liste pour voir si on a un résultat correct

        for (int i = 0; i < allCombinationsSO.allPossibleCombinations.Length; i++)
        {
            curCC = allCombinationsSO.allPossibleCombinations[i];
            if (ListIsCorrect(currentCombination, curCC.colorsToMix))
            {
                correct = true;
                index = i;
                break;
            }


        }

        //print(index);

        //Si on a un résultat correct, on ajoute la combinaison à la liste des mélanges réalisés
        if (correct && !allColorUIs[index].done)
        {

            for (int i = 0; i < allColorUIs.Length; i++)
            {
                allColorUIs[i].ValidateColor(curCC.colorResultID);
            }
            UpdateScoreUI();

            if (bolAnimCo != null)
                StopCoroutine(bolAnimCo);

            bolAnimCo = StartCoroutine(bolResultat.AddColorCo(curCC.colorResult));
            bolAnim.Play("a_mix_correct");

            validateBtn.interactable = false;
            currentCombination.Clear();

            if (!auto)
            {
                //Si on a pas terminé la liste
                if (curNbOfColorsCreated < allColorUIs.Length - 1)
                {
                    AudioManager.instance.Play(touilleClip);
                    AudioManager.instance.Play(goodClip);
                    curNbOfColorsCreated++;
                    correctParticle.SetActive(true);
                }
                //Sinon, on a réussi l'épreuve
                else
                {
                    AudioManager.instance.Play(victoryClip);
                    OnEpreuveEnded(true);
                    successParticle.SetActive(true);
                }
            }

        }
        //Sinon, on affiche le VFX d'erreur
        else
        {
            
                AudioManager.instance.Play(errorClip);
                AudioManager.instance.Play(videBolClip);
                erreurParticle.SetActive(true);
                ResetColors();
        }


        //On cache les traces de peinture
        StartCoroutine(ShowTracesPeintureCo(Color.clear));
    }





    //Permet de comparer notre combinaison actuelle avec la liste de tous les mélanges
    private bool ListIsCorrect(List<ColorID> compared, List<ColorID> listToProduce)
    {
        bool correct = true;

        if (listToProduce.Count == compared.Count)
        {
            compared.Sort();
            listToProduce.Sort();

            for (int i = 0; i < listToProduce.Count; i++)
            {
                if (!compared[i].Equals(listToProduce[i]))
                {
                    correct = false;
                    break;
                }
            }

        }
        else
        {
            correct = false;
        }

        return correct;

    }








    //Vérifie si les conditions de victoire sont réunies
    public void CheckVictory()
    {

        if (curNbOfColorsCreated < allColorUIs.Length - 1)
        {
            AudioManager.instance.Play(touilleClip);
            AudioManager.instance.Play(goodClip);
            curNbOfColorsCreated++;
            correctParticle.SetActive(true);
        }
        else
        {
            AudioManager.instance.Play(victoryClip);
            OnEpreuveEnded(true);
            successParticle.SetActive(true);
        }


    }




    #endregion




    #region Epreuves










    #endregion






    #region Overrides


    protected override IEnumerator Start()
    {



        //On setup le score et on initialise la combinaison actuelle

        scoreText.text = "0";
        finalScoreText.text = $"/{allColorUIs.Length}";

        currentCombination = new List<ColorID>();
        validateBtn.interactable = false;

        yield return StartCoroutine(base.Start());


    }


    //Quand on lance l'épreuve, on veut remplir automtiquement la première couleur automatiquement.
    //Ca permet d'indiquer au joueur ce qu'il doit faire et qu'il peut faire glisser les bols.
    protected override void OnEpreuveStarted()
    {
        base.OnEpreuveStarted();
        StartCoroutine(FillFirstColorCo());
    }






    private IEnumerator FillFirstColorCo()
    {
        //Pour l'anim d'intro on déplace chacun des bols un à un vers la calebasse.
        //Les couleurs seront automatiquement ajoutées au moment du trigger.
        //On attend que chaque bol soit retourné à son point de départ avant de passer à l'étape suivante.
        //Une fois les couleurs ajoutées, on valide la combinaison.

        clearButton.interactable = false;

        yield return StartCoroutine(MoveBolToCalebasse(firstAnimBol));
        yield return StartCoroutine(MoveBolToCalebasse(secondAnimBol));

        //Si le joueur n'a pas appuyé sur le bouton valider pdt l'anim, on peut mélanger
        if(currentCombination.Count > 1)
            ValidateCombination(false);


        //On débloque ensuite tous les bols
        BolDraggable[] bols = FindObjectsOfType<BolDraggable>();
        for (int i = 0; i < bols.Length; i++)
        {
            bols[i].introDone = true;
        }

        clearButton.interactable = true;



    }


    private IEnumerator MoveBolToCalebasse(BolDraggable bol)
    {
        bol.introDone = true;
        stopIntroAnim = false;
        PointerEventData ped = new PointerEventData(EventSystem.current) { position = bol.transform.position };

        bol.OnBeginDrag(ped);
        float timer = 0f;



        while (!stopIntroAnim && timer < 1f)
        {
            timer += 1f * Time.deltaTime;

            ped.position = Vector3.Lerp(bol.startPoint, new Vector3(Screen.width, Screen.height, bol.startPoint.z), timer);
            bol.MoveBolAnim(ped.position);
            yield return null;
        }

        bol.OnEndDrag(ped);


        //Tant que le bol est en animation, on délaie la fin de la coroutine avant d'enchaîner
        while (isAnimating)
        yield return null;

        bol.introDone = false;

    }





    //Appelle l'aide
    public override void GiveSolutionToPlayer(int index)
    {
        ResetHelpTimer();


        //On affiche une tâche de couleur au hasard dans la liste
        if (index < 2)
        {
            List<int> indices = new List<int>();

            for (int i = 0; i < allColorUIs.Length; i++)
            {
                if (allColorUIs[i].HasAtLeastTwoIngredientsInvisible())
                {
                    indices.Add(i);
                }
            }

            int alea = indices[Random.Range(0, indices.Count)];
            allColorUIs[alea].DisplayRandomTache();
        }


        //On complète une ligne au hasard
        else if (index < 4)
        {
            currentCombination.Clear();
            List<int> indices = new List<int>();

            for (int i = 0; i < allColorUIs.Length; i++)
            {
                if (!allColorUIs[i].done)
                {
                    indices.Add(i);
                }
            }

            int alea = indices[Random.Range(0, indices.Count)];

            ColorID[] colorsToMix = allCombinationsSO.GetCombinationFromColorResult(allColorUIs[alea].colorID);

            for (int i = 0; i < colorsToMix.Length; i++)
            {
                AddColorToMix(colorsToMix[i]);
            }

            ValidateCombination(true);
            CheckVictory();
        }


        //On complète tout
        else
        {
            currentCombination.Clear();

            for (int i = 0; i < allColorUIs.Length; i++)
            {
                if (!allColorUIs[i].done)
                {
                    ColorID[] colorsToMix = allCombinationsSO.GetCombinationFromColorResult(allColorUIs[i].colorID);

                    for (int j = 0; j < colorsToMix.Length; j++)
                    {
                        AddColorToMix(colorsToMix[j]);
                    }
                    ValidateCombination(true);
                }
            }

            curNbOfColorsCreated = allColorUIs.Length - 1;
            CheckVictory();
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
