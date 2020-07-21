using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableButtonBeforeEpreuve : MonoBehaviour
{
    private void OnLevelWasLoaded(int level)
    {
        if (level > 1 && level < 9)
            StartCoroutine(WaitForEpreuve());
        else
            GetComponent<Button>().interactable = true;
    }

    private IEnumerator WaitForEpreuve()
    {
        Epreuve e = FindObjectOfType<Epreuve>();
        Button b = GetComponent<Button>();

        b.interactable = false;

            while (e.EpreuveFinished)//Tant que l'épreuve n'a pas commencé
            {
                yield return null;
            }

            b.interactable = true;
            while (!e.EpreuveFinished)//Tant que l'épreuve ne s'est pas terminé
            {
                yield return null;
            }
            b.interactable = false;



    }
}
