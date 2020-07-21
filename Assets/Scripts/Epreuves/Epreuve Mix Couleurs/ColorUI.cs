using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorUI : MonoBehaviour
{
    public ColorID colorID;
    public bool shouldMaskElement, done;
    Image peintureImg, validationImg;
    public Image[] ingredientsToHide;


//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        Start();
//    }
//    private void Reset()
//    {
//        Start();
//    }
//#endif

    //[ContextMenu("Reveal All")]
    //public void RevealAll()
    //{

    //    ValidateColor(colorID);
    //}


    // Start is called before the first frame update
    void Start()
    {



        if (TryGetComponent(out peintureImg))
        {
            peintureImg.enabled = !shouldMaskElement;
        }


        if(peintureImg.transform.childCount > 0)
        if (peintureImg.transform.GetChild(0).TryGetComponent(out validationImg))
        {
            validationImg.enabled = done = false;
        }

        for (int i = 0; i < ingredientsToHide.Length; i++)
        {
            if (ingredientsToHide[i].TryGetComponent(out Image img))
            {
                img.enabled = false;
            }
        }
    }

    public void ValidateColor(ColorID resultID)
    {
        if (resultID != colorID)
            return;


        for (int i = 0; i < ingredientsToHide.Length; i++)
        {
            if (ingredientsToHide[i].TryGetComponent(out Image img))
            {
                img.enabled = true;
            }
        }

        if (TryGetComponent(out peintureImg))
        {
            peintureImg.enabled = true;
        }

        if(peintureImg.transform.childCount > 0)
            if (peintureImg.transform.GetChild(0).TryGetComponent(out validationImg))
            {
                validationImg.enabled = done = true;
            }
    }


    public void DisplayRandomTache()
    {
        int alea = Random.Range(0, ingredientsToHide.Length);

        for (int i = 0; i < ingredientsToHide.Length; i++)
        {
            if (ingredientsToHide[i].TryGetComponent(out Image img) && i == alea)
            {
                img.enabled = true;
            }
        }
    }

    public bool HasAtLeastTwoIngredientsInvisible()
    {
        int nbInvisible = 0;

        for (int i = 0; i < ingredientsToHide.Length; i++)
        {
            if (ingredientsToHide[i].TryGetComponent(out Image img))
            {
                if (!img.enabled)
                    nbInvisible++;
            }
        }

        return nbInvisible > 1;
    }
}
