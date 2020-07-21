using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "new Oeuvre", menuName = "Oeuvre", order = 3)]
public class Oeuvre : ScriptableObject
{
    public bool unlocked = false;
    public Sprite img;

    [Space(10)]

    public string title, date, author, materials;
    [TextArea(3, 10)] public string reference;

    [Space(10)]

    public string questionG;
    [TextArea(3, 10)] public string descriptionG;

    [Space(10)]

    public string questionD;
    [TextArea(3, 10)] public string descriptionD;
}
