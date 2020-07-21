using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* A placer sur un GameObject vide dans les scènes des Epreuves.
 * Ce script est utilisé pour lancer un dialogue au retour sur la scène ppale.
 * Cela nous permet de changer de dialogue en fonction de l'épreuve que l'on a terminé,
 * ou de lancer le dialogue d'intro si lancé depuis le menu ppal.
 */

public class MainSceneDialogueList : MonoBehaviour
{
    [Tooltip("Le dialogue à jouer une fois arrivé sur la scène principale.")]
    public DialogueList newDialogueDiscussionList;

    public static MainSceneDialogueList instance;


    //Singleton
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

    }


    //Lance le dialogue au retour sur la scène ppale
    private void OnLevelWasLoaded(int level)
    {
        //Si on passe l'intro, on se débarrasse de ce script. Sinon on joue le dialogue
        if (level == 1)
        {
            if (!FindObjectOfType<AnimIntro>().bypassIntro)
            {
                StartCoroutine(ReplaceDialogueTrigger());
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }



    private IEnumerator ReplaceDialogueTrigger()
    {
        //Tant que le système de dialogue n'est pas prêt,on ne fait rien.
        while(!DialogueDiscussionSystem.instance && !MainSceneButtons.instance)
        {
            yield return null;
        }

        //Sinon, on récupère le dialogue trigger et on lui passe notre liste de dialogue
        //pour récupérer le dialogue correspondant à la langue actuelle

        DialogueTrigger d = FindObjectOfType<DialogueTrigger>();
        d.dialogueType = DialogueTrigger.DialogueType.Discussion;
        d.PlayNewDialogue(newDialogueDiscussionList);

        Destroy(gameObject);
    }
}
