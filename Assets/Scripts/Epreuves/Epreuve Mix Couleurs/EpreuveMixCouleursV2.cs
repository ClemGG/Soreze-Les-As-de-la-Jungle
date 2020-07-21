using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EpreuveMixCouleursV2 : Epreuve
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    [Tooltip("L'animator pour faire bouger le bol")]
    [SerializeField] Animator bolAnim;


    [Tooltip("Le script responsable du mélange des couleurs")]
    public ColorCheckerV3 bolResultat;

    [Tooltip("Les particules à afficher qd un mélange est réalisé")]
    public GameObject correctParticle, successParticle, erreurParticle;

    [Space(10)]

    [Tooltip("Le ScriptableObject contenant la liste des combinaisons de couleur à réaliser")]
    public ColorCombinationScriptableObject allCombinationsSO;
    public Button validateBtn;

    [Space(10)]

    public Canvas canvasBols;
    public Canvas canvasEpreuve;


    [Space(10)]

    public bool isDragging = false;
    public bool isAnimating = false;

    [Tooltip("Le point où doivent s'arrêter les bols avant de verser leur couleur")]
    public RectTransform animEndPoint; 

    [Tooltip("Le sprite avant de la calebasse. Utilisé pour afficher correctement les effets de particule")]
    public Canvas calebasseFront;

    [Space(10)]
    [Header("Colors : ")]
    [Space(10)]

    int currentColorToCreateIndex = 0;
    int curNbOfColorsCreated = 0;

    [Space(10)]

    [Tooltip("Les icônes des résultats finaux sur la liste de couleurs en arrière-plan")]
    [SerializeField] ColorUI[] allColorUIs;

    [Space(10)]

    [Tooltip("Variable exposée affichant la combinaison actuellement dans le bol.")]
    public List<ColorID> currentCombination;


    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip bgmClip;
    public AudioClip goodClip;
    public AudioClip errorClip;
    public AudioClip victoryClip;

    public AudioClip touilleClip;
    public AudioClip verseClip;
    public AudioClip videBolClip;



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
    }


    //Appelée par le bouton reset
    public void ResetColors() 
    {
        bolAnim.Play("errorV2");
        validateBtn.interactable = false;

        currentCombination.Clear();
        bolResultat.ResetFioleColor();
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

    Coroutine bolAnimCo;
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
                bolAnim.Play("laineV2");
            //}

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


    //Vérifie si les conditions de victoire sont réunies
    }




    #endregion




    #region Epreuves



    public Color GetColorFromID(ColorID newID)
    {
        return allCombinationsSO.GetColorFromID(newID);
    }










    #endregion






    #region Overrides


    protected override IEnumerator Start()
    {
        //On setup le score et on initialise la combinaison actuelle


        scoreText.text = "0";
        finalScoreText.text = $"/{allCombinationsSO.allPossibleCombinations.Length}";

        yield return StartCoroutine(base.Start());

        currentCombination = new List<ColorID>();
        validateBtn.interactable = false;

    }



    //Appelle l'aide
    public override void GiveSolutionToPlayer(int index)
    {
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

            curNbOfColorsCreated++;
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
