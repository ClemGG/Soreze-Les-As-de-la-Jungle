using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    public bool done = false;
    public bool autoEnd = false;
    public float delayAutomaticClosure;

    [Space(10)]

    public Replique[] repliques;
}

public enum Mood { Normal, Etonnement, Réflexion, Détresse, Combat, Joie, Fierté, Crainte, Vaincu, Enfermé };
public enum Side { Left, Right };
public enum BubbleSize { Big, Medium, Small, OneLine };
public enum ArrowOrientation { Up, Down };

public delegate void OnReplique();


[System.Serializable]
public class Replique
{
    [TextArea(3, 10)] public string text;

    [Space(10)]

    public Sprite backgroundImg;
    public Character character;
    public AudioClip clip;

    [Space(10)]


    public Mood mood;
    public Side side;
    public BubbleSize bubbleSize;
    public ArrowOrientation arrowOrientation;

    public OnReplique onRepliqueStarted;
    public OnReplique onRepliqueEnded;

}


