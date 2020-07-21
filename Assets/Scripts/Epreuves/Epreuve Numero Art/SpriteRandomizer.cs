using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteRandomizer : MonoBehaviour
{

    public Sprite[] sprites;
    public bool useForUI = false;


    [ContextMenu("Set Random Sprite")]
    // Start is called before the first frame update
    void OnEnable()
    {
        if (useForUI)
        {
            GetComponent<Image>().sprite = sprites[Random.Range(0, sprites.Length)];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }

}
