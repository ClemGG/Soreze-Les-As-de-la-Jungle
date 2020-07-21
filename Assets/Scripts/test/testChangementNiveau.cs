using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testChangementNiveau : MonoBehaviour
{
    public bool bypass = false;
    public int levelToLoad = 0;
    public DialogueList unlockPhotoDialogList;

    private void OnMouseDown()
    {
        if (!MainSceneButtons.instance.introDone)
            return;

        if (!bypass)
        {

            bool allDone = true;
            if(levelToLoad == 9)
            {

                for (int i = 0; i < 6; i++)
                {
        
                    if(PlayerPrefs.GetInt($"EpreuveVictory{i}") == 0)
                    {
                        allDone = false;
                        break;
                    }
                }

            }
            if (allDone)
            {
                if (MainSceneButtons.instance.introDone)
                {
                    //SceneManager.LoadScene(levelToLoad);
                    DialogueEpreuveSystem.instance.StopDialogue();
                    ScreenTransitionImageEffect.instance.FadeToScene(levelToLoad);
                }
            }
            else
            {
                DialogueTrigger d = FindObjectOfType<DialogueTrigger>();
                d.dialogueType = DialogueTrigger.DialogueType.Epreuve;
                d.PlayNewDialogue(unlockPhotoDialogList);
            }
        }
        else
        {

            if (MainSceneButtons.instance.introDone)
            {
                //SceneManager.LoadScene(levelToLoad);
                DialogueEpreuveSystem.instance.StopDialogue();
                ScreenTransitionImageEffect.instance.FadeToScene(levelToLoad);
            }
        }



    }


}
