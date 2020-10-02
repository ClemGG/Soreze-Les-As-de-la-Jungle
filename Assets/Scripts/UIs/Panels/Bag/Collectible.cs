using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "new Collectible", menuName = "Collectible", order = 4)]
public class Collectible : ScriptableObject
{
    public bool unlocked = false;
    public Sprite img;
    public string itemName;

    [TextArea(3, 10)] public string description;

}
