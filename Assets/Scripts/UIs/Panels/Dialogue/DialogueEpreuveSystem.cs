using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueEpreuveSystem : MonoBehaviour
{

    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public GameObject dialoguePanel;
    public static DialogueEpreuveSystem instance;

    [Space(10)]
    [Header("UIs : ")]
    [Space(10)]


    public GameObject continueArrow;


    [Space(10)]

    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueText;

    [Space(10)]

    public Image characterImg;


    [Space(10)]
    [Header("Dialogue : ")]
    [Space(10)]

    public float writeSpeed = .05f;

    [Space(10)]

    int currentIndex = 0;
    [HideInInspector] public bool isWriting;
    [HideInInspector] public bool isPlaying;
    [HideInInspector] public Dialogue currentDialogue;

    //Appelées une fois le dialogue terminé
    public delegate void OnDialogueEnded();
    public OnDialogueEnded onDialogueEnded;


    private void OnLevelWasLoaded(int level)
    {
        onDialogueEnded = null;
        MainSceneButtons.instance.TogglePanel(dialoguePanel);
    }

    //Singleton
    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }



    // Appelée depuis le script DialogueTrigger, lui-même appelée une fois l'animation show du dialogue se termine
    public void StartDialogue(Dialogue dialogue)
    {
        currentIndex = 0;
        dialogue.done = false;
        currentDialogue = dialogue;
        isPlaying = true;

        StopAllCoroutines();
        StartCoroutine(DisplayDialogue());
    }


    //Appelée pour jouer la réplique suivvante
    public void ReadReplique(Replique replique)
    {
        currentIndex = 0;
        currentDialogue = null;

        StopAllCoroutines();
        StartCoroutine(DisplayReplique(replique));
    }

    public void StopDialogue()
    {

        StopAllCoroutines();
        if(dialoguePanel.activeSelf)
        MainSceneButtons.instance.TogglePanel(dialoguePanel);
    }





    public void NextReplique()
    {
        //Si on clique sur continuer alors qu'on est en train d'écrire le texte, on l'écrit tout en une fois
        if (isWriting)
        {
            StopAllCoroutines();
            WriteAlltext();
        }

        //Sinon, on écrit le texte un caractère à la fois
        else
        {
            //Tant que le dialogue n'est pas fini, on affiche la réplique suivante
            if (currentDialogue)
            {
                currentDialogue.repliques[currentIndex].onRepliqueEnded?.Invoke();

                if (!currentDialogue.done)
                {
                    currentIndex++;
                    StartCoroutine(DisplayDialogue());
                }
                else
                {
                    //Quand le dialogue est terminé
                    onDialogueEnded?.Invoke();
                    isPlaying = false;
                    MainSceneButtons.instance.TogglePanel(dialoguePanel);
                }
            }
            else if(currentReplique != null)
            {
                currentReplique.onRepliqueEnded?.Invoke();
                MainSceneButtons.instance.TogglePanel(dialoguePanel);
            }
        }
    }


    //Ecrit tout le texte de la réplique en cours d'un coup
    public void WriteAlltext()
    {
        Replique r = null;
        
        isWriting = false;
        continueArrow.SetActive(true);

        if (currentDialogue)
        {
            r = currentDialogue.repliques[currentIndex];
        }
        else if(currentReplique != null)
        {
            r = currentReplique;
        }

            dialogueText.text = r.text;

            characterImg.sprite = r.character.characterMedaillon;

        if(currentDialogue)
            currentDialogue.done = currentIndex == currentDialogue.repliques.Length - 1;
        
    }


    //Ecrit chaque caractère un à un de la réplique en cours
    private IEnumerator DisplayDialogue()
    {
        dialogueText.text = null;
        isWriting = true;


        Replique r = currentDialogue.repliques[currentIndex];

        //Exécute une fonction au début de la réplique s'il y en a une assignée
        r.onRepliqueStarted?.Invoke();


        //On affiche le personnage en cours
        characterImg.sprite = r.character.characterMedaillon;
        characterName.text = r.character.characterName;

        continueArrow.SetActive(false);


        //Si la réplique a un clip sonore on le joue
        if (r.clip)
        {
            AudioManager.instance.Play(r.clip);
        }


        //Pour écrire le texte, on utilise un StringBuilder pour nous permettre d'ajouter les caractères
        //les uns à la suite des autres et les écrire tous d'un coup.
        //C'est plus performant que de le faire à la main.

        float t = 0f;
        int stringIndex = 0;
        StringBuilder sb = new StringBuilder(500);

        while (isWriting)
        {

            if (t < writeSpeed)
            {
                t += Time.deltaTime;
            }
            else
            {
                t = 0f;
                sb.Append(r.text[stringIndex]);
                stringIndex++;

                dialogueText.text = sb.ToString();

                if (stringIndex == r.text.Length)
                {
                    isWriting = false;
                    continueArrow.SetActive(true);
                }

            }

            yield return null;
        }


        currentDialogue.done = currentIndex == currentDialogue.repliques.Length - 1;

        //On écrit la réplique suivante
        if (currentDialogue.done && currentDialogue.autoEnd)
        {
            yield return new WaitForSeconds(currentDialogue.delayAutomaticClosure);
            NextReplique();
        }

    }



    //Fait la même chose que DisplayDialogue() mais écrit uniquement une suele réplique.
    //Utilisé dans les épreuves pour afficher une consign eou un message d'erreur.

    Replique currentReplique;
    private IEnumerator DisplayReplique(Replique r)
    {
        dialogueText.text = null;
        isWriting = true;
        currentReplique = r;

        r.onRepliqueStarted?.Invoke();

        characterImg.sprite = r.character.characterMedaillon;
        characterName.text = r.character.characterName;

        continueArrow.SetActive(false);


        if (r.clip)
        {
            AudioManager.instance.Play(r.clip);
        }



        float t = 0f;
        int stringIndex = 0;
        StringBuilder sb = new StringBuilder(500);

        while (isWriting)
        {

            if (t < writeSpeed)
            {
                t += Time.deltaTime;
            }
            else
            {
                t = 0f;
                sb.Append(r.text[stringIndex]);
                stringIndex++;

                dialogueText.text = sb.ToString();

                if (stringIndex == r.text.Length)
                {
                    isWriting = false;
                    continueArrow.SetActive(true);
                }

            }

            yield return null;
        }


    }

}

