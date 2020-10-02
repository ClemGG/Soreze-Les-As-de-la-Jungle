using UnityEngine;

public class ComparaisonSprite : MonoBehaviour
{


    [HideInInspector] public int ID;
    [HideInInspector] public bool done = false;
    [HideInInspector] public SpriteRenderer sr;


    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        done = false;

        //Car le gameObject de la toile est au 1er index pour bloquer par défaut le projectile si on rate un trou
        ID = transform.GetSiblingIndex() - 1;
    }

    public void RevealSprite()
    {
        done = true;
        sr.enabled = true;
    }


}
