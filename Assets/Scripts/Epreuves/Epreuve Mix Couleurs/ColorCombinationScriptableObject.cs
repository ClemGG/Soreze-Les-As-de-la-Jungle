using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color Combination", menuName = "Color Combination")]
public class ColorCombinationScriptableObject : ScriptableObject
{
    public ColorCombination[] allPossibleCombinations;




    public ColorID[] GetCombinationFromColorResult(ColorID result)
    {
        for (int i = 0; i < allPossibleCombinations.Length; i++)
        {
            if(allPossibleCombinations[i].colorResultID == result)
            {
                return allPossibleCombinations[i].colorsToMix.ToArray();
            }
        }

        return null;
    }

    public Color GetColorFromID(ColorID newID)
    {
        if (newID == ColorID.Null)
            return Color.white;


        foreach (ColorCombination cc in allPossibleCombinations)
        {
            if (cc.colorsToMix.Count == 1)
            {
                if (cc.colorsToMix[0] == newID)
                {
                    return cc.colorResult;
                }
            }
        }

        return Color.white; //Renvoie du blanc en cas d'erreur
    }




    public bool MatchesID(List<ColorID> list, ColorID idToReach)
    {

        list.Sort();

        List<ColorCombination> allDuplicates = new List<ColorCombination>();

        foreach (ColorCombination cc in allPossibleCombinations)
        {
            if (cc.colorResultID == idToReach)
                allDuplicates.Add(cc);
        }
        if (allDuplicates.Count > 0)
        {
            foreach (ColorCombination cc in allDuplicates)
            {
                cc.colorsToMix.Sort();

                bool correct = true;

                for (int i = 0; i < cc.colorsToMix.Count; i++)
                {
                    if (cc.colorsToMix[i] != list[i])
                    {
                        correct = false;
                        break;
                    }
                }


                if (correct)
                {
                    return true;
                }
            }
        }

        return false;
    }





    public Color GetColorFromCombination(List<ColorID> list, ColorCombination colorToReach)
    {
        list.Sort();

        List<ColorCombination> allDuplicates = new List<ColorCombination>();

        foreach (ColorCombination cc in allPossibleCombinations)
        {
            if (cc.colorResult == colorToReach.colorResult)
                allDuplicates.Add(cc);
        }

        if (allDuplicates.Count > 0)
        {
            foreach (ColorCombination cc in allDuplicates)
            {
                cc.colorsToMix.Sort();

                if (cc.colorsToMix == list)
                {
                    return cc.colorResult;
                }
            }
        }
        return Color.white; //Renvoie du blanc en cas d'erreur

    }



    //public Color AverageColors(List<ColorID> list, Color currentColor)
    //{
    //    Color lastColor = Color.white;

    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        switch (list[i])
    //        {
    //            case ColorID.Rouge:
    //                currentColor = i == 0 ? Color.red : currentColor.LerpColor(lastColor, Color.red);
    //                break;

    //            case ColorID.Vert:
    //                currentColor = i == 0 ? Color.yellow : currentColor.LerpColor(lastColor, Color.yellow);
    //                break;

    //            case ColorID.Bleu:
    //                currentColor = i == 0 ? Color.blue : currentColor.LerpColor(lastColor, Color.blue);
    //                break;

    //            case ColorID.Noir:
    //                currentColor = i == 0 ? Color.black : new Color(currentColor.r - .33f, currentColor.g - .33f, currentColor.b - .33f);
    //                break;

    //            case ColorID.Blanc:
    //                currentColor = i == 0 ? Color.white : new Color(currentColor.r + .33f, currentColor.g + .33f, currentColor.b + .33f);
    //                break;
    //        }

    //        lastColor = currentColor;
    //    }


    //    return currentColor;
    //}

}
