using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using static Clement.Utilities.Textures;
using UnityEngine.XR.ARFoundation;

public class DialogueDiscussionSystem : MonoBehaviour
{

    #region Variables


    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]



    public GameObject dialoguePanel;
    public static DialogueDiscussionSystem instance;
    public Material blendTextureMaterial;

    [Space(10)]
    public int nbTotalEpreuves = 6;

    [Tooltip("Le dialogue qui se joue quand le joue déploque une première oeuvre")]
    public DialogueList dialogueListFirstOeuvre;
    bool firstOeuvreDone = false;



    [Space(10)]
    [Header("UIs : ")]
    [Space(10)]

    public CanvasGroup persoLeftAlpha;
    public CanvasGroup persoRightAlpha;
    public CanvasGroup bubbleLeftAlpha;
    public CanvasGroup bubbleRightAlpha;

    [Space(10)]

    public TextMeshProUGUI dialogueTextLeft;
    public TextMeshProUGUI dialogueTextRight;

    [Space(10)]

    public Image characterLeftImg;
    public Image characterRightImg;
    public Image backgroundImg;
    public GameObject previousButton;






    [Space(10)]
    [Header("Dialogue : ")]
    [Space(10)]

    [Tooltip("Vitesse de fondu des persos et des bulles")]
    public float fadeSpeed = 2f;
    public AnimationCurve fadeCurve;

    [Space(10)]

    int currentIndex = 0;

    bool isFading, leftHasAppeared, rightHasAppeared;
    Side lastSide;
    Mood lastMood;
    Character lastChar;
    public Dialogue currentDialogue;

    //Appelées une fois le dialogue terminé
    public delegate void OnDialogueEnded();
    public OnDialogueEnded onDialogueEnded;







    [Space(10)]
    [Header("Bulles : ")]
    [Space(10)]

    public RectTransform bubbleLeft;
    public RectTransform bubbleRight;
    public Vector2[] allBulleSizes;


    #endregion


    #region Mono

    private void OnLevelWasLoaded(int level)
    {
        onDialogueEnded = null;
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
        backgroundImg.material = new Material(blendTextureMaterial);

    }
    #endregion


    #region Dialogue


    // Appelée depuis le script DialogueTrigger, lui-même appelée une fois l'animation show du dialogue se termine
    public void StartDialogue(Dialogue dialogue)
    {
        previousButton.SetActive(false);

        //On initialise les paramètres et on lance la première réplique si le dialogue n'est pas vide
        currentIndex = 0;
        rightHasAppeared = leftHasAppeared = dialogue.done = false;

        currentDialogue = dialogue;
        if(currentDialogue.repliques.Length == 0 || currentDialogue == null)
        {
            //Si on quitte un mini-jeu, on repart dans la scène ppale avec un dialogue vide.
            //Donc on regarde si on a réussi la 1ère oeuvre et on affiche le dialogue de la 1ère oeuvre.
            EndEpreuve();

        }
        else
        {
            lastSide = currentDialogue.repliques[0].side;
            lastMood = currentDialogue.repliques[0].mood;
            lastChar = currentDialogue.repliques[0].character;

            StopAllCoroutines();
            StartCoroutine(DisplayDialogue());
        }

    }


    //Appelée pour jouer la réplique suivvante
    public void NextReplique()
    {


        if (isFading || ScreenTransitionImageEffect.instance.isTransitioning)
            return;

        previousButton.SetActive(true);


        StopCoroutine(DisplayDialogue());
        currentDialogue.repliques[currentIndex].onRepliqueEnded?.Invoke();

        //S'il reste des répliques, on joue la suivante
        if (!currentDialogue.done)
        {
            currentIndex++;
            StartCoroutine(DisplayDialogue());
        }
        else
        {
            //Sinon, on termine le dialogue
            EndEpreuve();
        }

    }


    //Appelée pour jouer la réplique précédente, si le joueur l'a passé trop vite
    public void PreviousReplique()
    {


        if (isFading || ScreenTransitionImageEffect.instance.isTransitioning)
            return;


        StopCoroutine(DisplayDialogue());

        //S'il reste des répliques à remonter, on joue la précédente
        if (currentIndex > 0)
        {
            currentIndex--;
            StartCoroutine(DisplayDialogue());
        }

        previousButton.SetActive(currentIndex != 0);

    }

    private void EndEpreuve()
    {
        onDialogueEnded?.Invoke();

        //Si le joueur a récupéré sa première oeuvre, on joue le dialogue correspondant.
        //Sinon, on cache le panel

        int nbEpreuvesDone = 0;

        for (int i = 0; i < nbTotalEpreuves; i++)
        {
            if (PlayerPrefs.HasKey($"EpreuveVictory{i}"))
            {
                if (PlayerPrefs.GetInt($"EpreuveVictory{i}", 0) == 1)
                {
                    nbEpreuvesDone++;
                }
            }
        }
        //print(nbEpreuvesDone);
        if (!firstOeuvreDone && nbEpreuvesDone == 1)
        {
            firstOeuvreDone = true;
            FindObjectOfType<DialogueTrigger>().PlayNewDialogue(dialogueListFirstOeuvre);

        }
        else
        {
            MainSceneButtons.instance.TogglePanel(dialoguePanel);
        }
    }







    Sprite lastBackground, currentBackground;

    private IEnumerator DisplayDialogue()
    {
        //On récupère la réplique en cours
        Replique r = currentDialogue.repliques[currentIndex];

        while (ScreenTransitionImageEffect.instance.isTransitioning)
            yield return null;


        isFading = true;
        float t = 0f;


        while (ScreenTransitionImageEffect.instance.isTransitioning)
            yield return null;


        //On affiche le background en fondu en fonction de qui parle et où

        if (currentIndex == 0)
        {
            lastBackground = Resources.Load<Sprite>("noir");
        }
        currentBackground = backgroundImg.sprite = r.backgroundImg;


        backgroundImg.material.SetTexture("_Texture1", lastBackground.ToTexture2D());
        backgroundImg.material.SetTexture("_Texture2", currentBackground.ToTexture2D());
        backgroundImg.material.SetFloat("_Blend", 0f);


        //On affiche le la bulle et son texte du côté du personnage qui parle


        if (r.side == Side.Right)
        {
            dialogueTextRight.text = r.text;
            characterRightImg.sprite = r.character.GetImageByMood(r.mood);


            bubbleRight.sizeDelta = allBulleSizes[(int)r.bubbleSize];


            bubbleRight.transform.GetChild(1).gameObject.SetActive(r.arrowOrientation == ArrowOrientation.Up);
            bubbleRight.transform.GetChild(2).gameObject.SetActive(r.arrowOrientation == ArrowOrientation.Down);
        }
        else
        {
            dialogueTextLeft.text = r.text;
            characterLeftImg.sprite = r.character.GetImageByMood(r.mood);


            bubbleLeft.sizeDelta = allBulleSizes[(int)r.bubbleSize];

            bubbleLeft.transform.GetChild(1).gameObject.SetActive(r.arrowOrientation == ArrowOrientation.Up);
            bubbleLeft.transform.GetChild(2).gameObject.SetActive(r.arrowOrientation == ArrowOrientation.Down);
        }



        //Pour alterner entre les personnages
        if (!rightHasAppeared)
        {
            persoRightAlpha.alpha = 0f;

        }
        if (!leftHasAppeared)
        {
            persoLeftAlpha.alpha = 0f;
        }


        //Si la réplique a un clip sonore on le joue
        if (r.clip)
        {
            AudioManager.instance.Play(r.clip);
        }





        //Cette boucle change l'alpha du texte fonction de la bulle correspondante


        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            float a = fadeCurve.Evaluate(t);

            backgroundImg.material.SetFloat("_Blend", a);


            if (r.side == Side.Right)
            {
                if(( (lastSide != r.side || lastMood != r.mood) && lastChar == r.character) || lastChar != r.character || currentIndex == 0)
                    persoRightAlpha.alpha = a;

                if(lastSide != r.side)
                    persoLeftAlpha.alpha = 1f-a;

                dialogueTextRight.color = new Color(dialogueTextRight.color.r, dialogueTextRight.color.g, dialogueTextRight.color.b, a);

            }
            else
            {
                if(lastSide != r.side)
                 persoRightAlpha.alpha = 1f-a;

                if (( (lastSide != r.side || lastMood != r.mood) && lastChar == r.character) || lastChar != r.character || currentIndex == 0)
                    persoLeftAlpha.alpha = a;

                dialogueTextLeft.color = new Color(dialogueTextLeft.color.r, dialogueTextLeft.color.g, dialogueTextLeft.color.b, a);

            }

            //Si on change de bulle ou que c'est la première du dialogue, on change son alpha
            if(currentIndex == 0 || lastSide != r.side)
            {
                if (r.side == Side.Right)
                {
                    bubbleLeftAlpha.alpha = 0f;
                    bubbleRightAlpha.alpha = a;
                }
                else
                {
                    bubbleLeftAlpha.alpha = a;
                    bubbleRightAlpha.alpha = 0f;
                }
            }


            yield return null;
        }

        //On affiche le background en fondu en fonction de qui parle et où
        lastBackground = r.backgroundImg;

        backgroundImg.material.SetTexture("_Texture1", lastBackground.ToTexture2D());
        backgroundImg.material.SetTexture("_Texture2", currentBackground.ToTexture2D());
        backgroundImg.material.SetFloat("_Blend", 0f);


        //On assigne les nouveaux paramètres pour garder la trace du dernier personnage qui a parlé et de quel côté

        currentDialogue.done = currentIndex == currentDialogue.repliques.Length - 1;

        lastSide = r.side;
        lastMood = r.mood;
        lastChar = r.character;

        isFading = false;

        if (r.side == Side.Right && !rightHasAppeared)
        {
            rightHasAppeared = true;
        }
        else if (r.side == Side.Left && !leftHasAppeared)
        {
            leftHasAppeared = true;
        }


        while (ScreenTransitionImageEffect.instance.isTransitioning)
            yield return null;


        //On passe ensuite à la réplique suivante
        if (currentDialogue.done && currentDialogue.autoEnd)
        {
            WaitForSeconds wait = new WaitForSeconds(currentDialogue.delayAutomaticClosure);
            yield return wait;

            NextReplique();
        }
    }


    #endregion
}
