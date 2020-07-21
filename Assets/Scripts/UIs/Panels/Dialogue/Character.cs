using System;
using UnityEngine;


[Serializable]
[CreateAssetMenu(fileName = "new Character", menuName = "Character", order = 2)]
public class Character : ScriptableObject
{

    public string characterName;
    public Sprite characterMedaillon;
    public FacialExpression[] characterExpressions;



    public Sprite GetImageByMood(Mood mood)
    {
        FacialExpression expression = Array.Find(characterExpressions, e => e.mood == mood);
        return expression.sprite;
    }

}



[Serializable]
public struct FacialExpression
{
    public Mood mood;
    public Sprite sprite;
}