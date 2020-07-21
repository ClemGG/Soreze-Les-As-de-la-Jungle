using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EpreuveMixCouleurs : Epreuve
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    [SerializeField] ColorChecker bolResultat;
    [SerializeField] ColorChecker[] colorCheckers;
    [Space(10)]
    [SerializeField] GameObject[] bolsToSpawn;
    [Space(10)]
    public ColorCombinationScriptableObject allPossibleCombinations;
    public Button validateBtn;

    [Space(10)]


    [Space(10)]
    [Header("Materials : ")]
    [Space(10)]

    [SerializeField] Material bacLiquide;
    [SerializeField] Material laine;

    [Space(10)]
    [Header("Colors : ")]
    [Space(10)]

    public ColorID currentColorID = ColorID.Null;
    int currentColorToCreateIndex = 0;

    [Space(10)]
    ColorUI[] allColorUIs;
    [Space(10)]
    [SerializeField] ColorCombination[] colorsToCreate;
    List<ColorID> currentCombination;

    [Space(10)]
    public int nbErreurs = 0;
    [Space(10)]
    public int[] nbErreursIntervalle;


    #region Epreuve avec bols


    public void MixColors() { }
    public void ResetColors() { }
    public void AddColorToMix(int index) { }
    public void ValidateCombination(int index) { }
    public void AddColorToPalette(int index) { }




    #endregion




    #region Epreuves

    public void ChangeCurrentColor(int i)
    {
        if (EpreuveFinished)
            return;

        currentColorID = (ColorID)i;
    }


    public Color GetColorFromID(ColorID newID)
    {
        return allPossibleCombinations.GetColorFromID(newID);
    }






    //public void ValidateCombination()
    //{
    //    if (EpreuveFinished)
    //        return;




    //    List<ColorID> currentColorCombination = new List<ColorID>();
    //    for (int i = 0; i < colorCheckers.Length; i++)
    //    {
    //        if(colorCheckers[i].currentColorID != ColorID.Null)
    //            currentColorCombination.Add(colorCheckers[i].currentColorID);
    //    }

    //    if (currentColorCombination.Count == 0)
    //        return;


    //    if (allPossibleCombinations.MatchesID(currentColorCombination, colorsToCreate[currentColorToCreateIndex].colorResultID))
    //    {
    //        bacLiquide.color = colorsToCreate[currentColorToCreateIndex].colorResult;
    //        laine.color = colorsToCreate[currentColorToCreateIndex].colorResult;

    //        UnlockNewColorButton();
    //        currentColorToCreateIndex++;

    //        if (currentColorToCreateIndex == 0)
    //        {
    //            if (!dialogGoodAnswerGiven && dialogGoodAnswer.listeDialogueEpreuve.Count != 0)
    //            {
    //                EpreuveFinished = true;
    //                DialogueEpreuveSystem.instance.onDialogueEnded += OnEpreuveStarted;
    //                dialogueTrigger.PlayNewDialogue(dialogGoodAnswer);
    //                dialogGoodAnswerGiven = true;
    //            }
    //        }

    //    }
    //    else
    //    {
    //        bacLiquide.color = Color.white; //Renvoie un vert caca d'oie pour l'erreur
    //        laine.color = Color.white;

    //        //Erreur de combinaison
    //        nbErreurs++;
    //        for (int i = 0; i < nbErreursIntervalle.Length; i++)
    //        {
    //            if(nbErreurs == nbErreursIntervalle[i])
    //            {
    //                HelpPanelButtons.instance.btnCameleon.gameObject.SetActive(true);
    //            }
    //        }


    //        if (!dialogBadAnswerGiven && dialogBadAnswer.listeDialogueEpreuve.Count != 0)
    //        {

    //            dialogueTrigger.PlayNewDialogue(dialogBadAnswer);
    //            EpreuveFinished = true;
    //            DialogueEpreuveSystem.instance.onDialogueEnded += OnEpreuveStarted;
    //            dialogBadAnswerGiven = true;
    //        }

    //    }


    //    for (int i = 0; i < colorCheckers.Length; i++)
    //    {
    //        colorCheckers[i].ResetFioleColor();
    //    }



    //    if (currentColorToCreateIndex == colorsToCreate.Length)
    //    {
    //        OnEpreuveEnded(true);
    //    }
    //}

    private void UnlockNewColorButton()
    {
        if(currentColorToCreateIndex < bolsToSpawn.Length)
            bolsToSpawn[currentColorToCreateIndex].SetActive(true);
    }






    public void AddColorToCurrentCombination(ColorID col)
    {
        currentCombination.Add(col);
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


    //Appelée par le bouton de réinitialisation

    public void ResetBolResultat()
    {
        bolResultat.ResetFioleColor();
    }

    //Appelée par le bouton de mélange
    public void ValidateCombination()
    {
        bool correct = false;
        int index = -1;

        for (int i = 0; i < colorsToCreate.Length; i++)
        {
            if (currentCombination.Equals(colorsToCreate[i]))
            {
                correct = true;
                index = i;
                break;
            }
        }

        if (correct)
        {
            UpdateScoreUI();
            allColorUIs[index].ValidateColor(colorsToCreate[index].colorResultID);


            if (currentColorToCreateIndex < colorsToCreate.Length - 1)
                currentColorToCreateIndex++;
            else
                OnEpreuveEnded(true);
        }
        else
        {

        }

    }





    #endregion






    #region Overrides


    protected override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());

        for (int i = 0; i < bolsToSpawn.Length; i++)
        {
            bolsToSpawn[i].SetActive(false);
            yield return null;
        }

        allColorUIs = FindObjectsOfType<ColorUI>();
        scoreText.text = "0";
        finalScoreText.text = $"/{colorsToCreate.Length}";
    }


    protected override void Update()
    {
        validateBtn.interactable = !EpreuveFinished;

        for (int i = 0; i < colorCheckers.Length; i++)
        {
            if (colorCheckers[i].isAnimating)
            {
                validateBtn.interactable = false;
                break;
            }
            else if(!EpreuveFinished)
            {
                validateBtn.interactable = true;

            }
        }


    }



    protected override void OnEpreuveStarted()
    {
        base.OnEpreuveStarted();

        currentColorID = ColorID.Null;

    }

    public override void GiveSolutionToPlayer(int index)
    {

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
