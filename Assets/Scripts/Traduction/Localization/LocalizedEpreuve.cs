using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Epreuve Info", menuName = "Epreuve Info", order = 5)]
public class LocalizedEpreuve : ScriptableObject
{
    public string nomEpreuve;
    [TextArea(3, 10)] public string instructions;
}
