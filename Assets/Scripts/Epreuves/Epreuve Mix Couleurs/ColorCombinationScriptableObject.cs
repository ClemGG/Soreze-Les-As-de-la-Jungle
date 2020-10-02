using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color Combination", menuName = "Color Combination")]
public class ColorCombinationScriptableObject : ScriptableObject
{
    public ColorCombination[] allPossibleCombinations;
    public ColorAndID[] colorAndIDs;



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





    //Utilisée pour colorier les traces de la calebasse
    public Color GetColorFromID(ColorID newID)
    {
        if (newID == ColorID.Null)
            return Color.clear;

        for (int i = 0; i < colorAndIDs.Length; i++)
        {
            if (colorAndIDs[i].id == newID)
            {
                return colorAndIDs[i].color;
            }
        }

        return Color.clear; //Renvoie du blanc en cas d'erreur
    }





    public bool MatchesID(List<ColorID> list, ColorID idToReach)
    {

        list.Sort();

        List<ColorCombination> allDuplicates = new List<ColorCombination>();

        for (int i = 0; i < allPossibleCombinations.Length; i++)
        {
            if (allPossibleCombinations[i].colorResultID == idToReach)
                allDuplicates.Add(allPossibleCombinations[i]);
        }
        if (allDuplicates.Count > 0)
        {
            for (int i = 0; i < allPossibleCombinations.Length; i++)
            {
                allPossibleCombinations[i].colorsToMix.Sort();

                bool correct = true;

                for (int j = 0; j < allPossibleCombinations[i].colorsToMix.Count; i++)
                {
                    if (allPossibleCombinations[i].colorsToMix[j] != list[i])
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

        for (int i = 0; i < allPossibleCombinations.Length; i++)
        {
            if (allPossibleCombinations[i].colorResult == colorToReach.colorResult)
                allDuplicates.Add(allPossibleCombinations[i]);
        }

        if (allDuplicates.Count > 0)
        {
            for (int i = 0; i < allDuplicates.Count; i++)
            {
                allDuplicates[i].colorsToMix.Sort();

                if (allDuplicates[i].colorsToMix == list)
                {
                    return allDuplicates[i].colorResult;
                }
            }
        }
        return Color.white; //Renvoie du blanc en cas d'erreur

    }




}

[System.Serializable]
public struct ColorAndID
{
    public ColorID id;
    public Color color;
}