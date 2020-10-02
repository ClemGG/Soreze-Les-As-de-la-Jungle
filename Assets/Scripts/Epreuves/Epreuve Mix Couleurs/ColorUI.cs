using UnityEngine;
using UnityEngine.UI;

public class ColorUI : MonoBehaviour
{
    public ColorID colorID;
    public bool shouldMaskElement, done;
    Image peintureImg, validationImg;
    public Image[] ingredientsToHide;




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



    //Affiche les couleurs de tous les ingrédients d'une ligne réussie
    public void ValidateColor(ColorID resultID)
    {
        if (resultID != colorID)
            return;


        for (int i = 0; i < ingredientsToHide.Length; i++)
        {
            if (ingredientsToHide[i].TryGetComponent(out Image img))
            {
                img.enabled = true;
                img.GetComponent<Animator>().enabled = true;

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



    //Affiche une tâche pour aider le joueur
    public void DisplayRandomTache()
    {
        int alea = Random.Range(0, ingredientsToHide.Length);

        for (int i = 0; i < ingredientsToHide.Length; i++)
        {
            if (ingredientsToHide[i].TryGetComponent(out Image img) && i == alea)
            {
                img.enabled = true;
                img.GetComponent<Animator>().enabled = true;
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
