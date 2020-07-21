using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public enum DialogueType { Discussion, Epreuve };
    public DialogueType dialogueType;

    public DialogueList dialoguesToPlay;


    //A appeler depuis l'animation d'ouverture

    //private void OnEnable()
    //{
    //    PlayNewDialogue();

    //}

    [ContextMenu("Play New Dialogue")]
    public void PlayNewDialogue()
    {


        Dialogue dialogueToPlay = null;


        switch (PlayerPrefs.GetString("langue"))
        {
            case "fr":
                dialogueToPlay = dialoguesToPlay.listeDialogueEpreuve[0];
                break;

            case "en":
                dialogueToPlay = dialoguesToPlay.listeDialogueEpreuve[1];
                break;

            case "es":
                dialogueToPlay = dialoguesToPlay.listeDialogueEpreuve[2];
                break;

            default:
                print("Erreur dialogue : Aucune langue n'a été sélectionnée");
                break;

        }

        if (dialogueToPlay)
        {
            MainSceneButtons.instance.ToggleDialoguePanel(dialogueType);

            if (dialogueType == DialogueType.Discussion)
            {
                DialogueDiscussionSystem.instance.StartDialogue(dialogueToPlay);

                //print("Discussion : " + dialogueToPlay.name);
            }
            else
            {
                DialogueEpreuveSystem.instance.StartDialogue(dialogueToPlay);

                //print("Epreuve : " + dialogueToPlay.name);
            }

        }
    }




    public void PlayNewDialogue(DialogueList newDialogueList)
    {
        if (newDialogueList == null)
        {
            print($"Erreur : la liste de dialogues \"{nameof(newDialogueList)}\" est vide.");
            return;

        }

        dialoguesToPlay = newDialogueList;
        PlayNewDialogue();
    }
}
