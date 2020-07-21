using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string questionTag, helpTag; // Pour la traduction
    public int correctAnswer;
    public bool done = false;
}
