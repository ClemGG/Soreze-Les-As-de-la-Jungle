using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR.ARFoundation;

public class AnimIntro : MonoBehaviour
{
    #region Variables



    [Tooltip("Permet de passer l'intro pour les tests.")]
    public bool bypassIntro = false;

    [Space(20)]

    [Tooltip("Le Component qui se charge de l'animation d'intro.")]
    public PlayableDirector timeline;
    [Tooltip("Le dialogue trigger qui lancera le dialogue de tuto en bas de l'ecran.")]
    public DialogueTrigger tutoDialogueTrigger;

    [Tooltip("Le dialogue du tuto (en bas de l'écran) et le 2ème dialogue qui se joue après le tuto.")]
    public DialogueList tutoDialogueList, discussion2DialogueList;

    [Space(10)]


    [Tooltip("Pour savoir quel item faire apparaître pendant le dialogue de tuto")]
    public int tutoProgressionIndex = 0;

    [Tooltip("Le panel qui contient les cinématiques et les items du tuto.")]
    public GameObject panelIntro;

    [Tooltip("Les icones à faire spawner à chaque phrase du dialogue.")]
    public GameObject[] tutoObjects;

    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]


    [Tooltip("Le son de la cinématique à arrêter.")]
    [SerializeField] GameObject ambientJungleAudioSource;

    //Les systèmes de dialogue
    DialogueDiscussionSystem dd;
    DialogueEpreuveSystem de;

    #endregion


    private IEnumerator Start()
    {

        //Si on saute l'intro, on considère le tuto comme fini et on initialise les systèmes de dialogue
        if (bypassIntro)
        {
            MainSceneButtons.instance.introDone = true;
            HelpPanelButtons.instance.cameleon.SetActive(false);

            //On récupère les systèmes de dialogue de cette façon car leurs panels seront désacivés au lancement de la scène ppale.
            //Cette fonction est le seul moyen de les retrouver par script
            DialogueEpreuveSystem[] DES = (DialogueEpreuveSystem[])FindObjectsOfTypeAll(typeof(DialogueEpreuveSystem));
            DialogueDiscussionSystem[] DDS = (DialogueDiscussionSystem[])FindObjectsOfTypeAll(typeof(DialogueDiscussionSystem));

            DES[0].gameObject.SetActive(true);
            DDS[0].gameObject.SetActive(true);
            yield return null;
            DES[0].gameObject.SetActive(false);
            DDS[0].gameObject.SetActive(false);

            yield break;
        }

        //On reset les paramètres du canvas ppal que l'on a dû modifier pour le fondu en noir
        tutoDialogueTrigger.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        tutoDialogueTrigger.GetComponent<Canvas>().planeDistance = 10f;

        //On active le caméléon pour le tuto si on ne passe pas l'intro
        if (!MainSceneButtons.instance.introDone)
            HelpPanelButtons.instance.cameleon.SetActive(true);


        //On active le panel de la cinématique et on cache les icônes du tuto
        panelIntro.SetActive(true);
        for (int i = 0; i < tutoObjects.Length; i++)
        {
            if(tutoObjects[i])
            tutoObjects[i].SetActive(false);
        }

        //On joue la cinématique
        timeline.Play();

        //On garde en mémoire les systèmes de dialogues pour y accéder plus facilement
        DialogueDiscussionSystem dd = DialogueDiscussionSystem.instance;
        DialogueEpreuveSystem de = DialogueEpreuveSystem.instance;

        //On indique au dialogue en plein écran de jouer le dialogue de tuto
        //une fois que le premier dialogue d'intro sera terminé
        dd.onDialogueEnded += EnableEpreuveDialogueAfterDiscussionDialogueFade;
    }





    public void EnableEpreuveDialogueAfterDiscussionDialogueFade()
    {
        //Active les icônes de tuto
        StartCoroutine(WaitFadeCo());


        dd = DialogueDiscussionSystem.instance;
        dd.onDialogueEnded -= EnableEpreuveDialogueAfterDiscussionDialogueFade;
    }








    //Appelée une fois le dialogue en bas de l'écran lancé pour activer une des icônes du tuto à chaque clic.
    //On assigne chaque icône à une réplique quiva se charger d'appeler la fonction DisplayNextTutoObject() une fois lue
    private IEnumerator WaitFadeCo()
    {
        yield return new WaitForSeconds(1.5f);
        tutoDialogueTrigger.dialogueType = DialogueTrigger.DialogueType.Epreuve;

        for (int i = 0; i < tutoDialogueList.listeDialogueEpreuve.Count; i++)
        {
            tutoDialogueList.listeDialogueEpreuve[i].repliques[0].onRepliqueStarted += DisplayNextTutoObject;
            yield return null;
        }

        tutoDialogueTrigger.PlayNewDialogue(tutoDialogueList);

        de = DialogueEpreuveSystem.instance;

        for (int i = 1; i < de.currentDialogue.repliques.Length; i++)
        {
            de.currentDialogue.repliques[i].onRepliqueStarted += DisplayNextTutoObject;
            yield return null;
        }


        //Une fois les icônes assignées à chaque réplique
        de.currentDialogue.repliques[de.currentDialogue.repliques.Length-1].onRepliqueEnded += DisplayNextTutoObject;
        de.currentDialogue.repliques[de.currentDialogue.repliques.Length-1].onRepliqueEnded += PlayNextDiscussion;

    }


    //Appelée par les répliques du tuto pour afficher ou cacher les icônes du tuto
    private void DisplayNextTutoObject()
    {
        for (int i = 0; i < tutoObjects.Length; i++)
        {
            if (tutoObjects[i])
                tutoObjects[i].SetActive(i == tutoProgressionIndex);
        }
        tutoProgressionIndex++;

    }


    //Appelée à la fin du dialogue tuto pour jouer la 2ème discussion en plein écran
    private void PlayNextDiscussion()
    {
        for (int i = 0; i < tutoDialogueList.listeDialogueEpreuve.Count; i++)
        {
            tutoDialogueList.listeDialogueEpreuve[i].repliques[0].onRepliqueStarted -= DisplayNextTutoObject;
        }

        StartCoroutine(WaitFade2Co());
    }

    //Initialise le système de dialogue et indique à la dernière réplique de terminer l'intro
    private IEnumerator WaitFade2Co()
    {

        //yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < de.currentDialogue.repliques.Length; i++)
        {
            de.currentDialogue.repliques[i].onRepliqueStarted -= DisplayNextTutoObject;
            yield return null;
        }
        de.currentDialogue.repliques[de.currentDialogue.repliques.Length - 1].onRepliqueEnded -= DisplayNextTutoObject;


        tutoDialogueTrigger.dialogueType = DialogueTrigger.DialogueType.Discussion;
        tutoDialogueTrigger.PlayNewDialogue(discussion2DialogueList);

        de = DialogueEpreuveSystem.instance;
        de.currentDialogue.repliques[de.currentDialogue.repliques.Length - 1].onRepliqueEnded -= PlayNextDiscussion;


        dd = DialogueDiscussionSystem.instance;
        dd.currentDialogue.repliques[dd.currentDialogue.repliques.Length - 1].onRepliqueEnded += EndIntro;
    }


    private void EndIntro()
    {
        //L'intro est finie

        MainSceneButtons.instance.introDone = true;

        //On cache le caméléon pour ne l'afficher que dans les épreuves
        HelpPanelButtons.instance.cameleon.SetActive(false);

        panelIntro.SetActive(false);
        dd = DialogueDiscussionSystem.instance;
        dd.currentDialogue.repliques[dd.currentDialogue.repliques.Length - 1].onRepliqueEnded -= EndIntro;

        //On coupe l'audio de la jungle
        Destroy(ambientJungleAudioSource);
    }

}

