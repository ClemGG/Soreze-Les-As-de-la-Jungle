using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PanelPhotoButtons : MonoBehaviour
{

    #region Variables

    public Canvas uiProtoCanvas;
    public Button homeButton;

    [Space(10)]

    [Tooltip("Les panels à (dés)activer à chaque étape du process.")]
    public GameObject[] panels;

    [Space(10)]

    [Tooltip("Les boutons du canvas ppal à désactiver.")]
    public GameObject[] buttonsToDisableOnEnable;





    [Space(10)]
    [Header("Reticule : ")]
    [Space(10)]

    public GameObject reticleProfile;
    public Button reticleBtn;
    public Image reticleCircle, fadePhotoImg;
    public Text reticleText;

    [Space(10)]

    [Tooltip("Le timer avant la prise de photo.")]
    public float countdown = 5f;
    float timer;
    [HideInInspector] bool countdownHasStarted = false;







    [Space(10)]
    [Header("Motifs : ")]
    [Space(10)]

    public Scrollbar scrollBarMotifs;
    public int maxMotifs = 10;
    [HideInInspector] public int motifCount;
    [HideInInspector] public List<GameObject> addedMotifs;






    [Space(10)]
    [Header("Print : ")]
    [Space(10)]

    //public InputField emailField;
    [Tooltip("La photo en elle-même, qui contiendra les motifs à la fin de l'édition.")]
    public Image photoImg;
    Texture2D photoTex; //Contiendra temporairement la photo qui sera ensuite détruite
    string path;

    [Space(10)]

    public GameObject panelLoading;
    public GameObject panelPrintDone;

    [Space(10)]
    [Header("Photo : ")]
    [Space(10)]






    [Tooltip("Affiche l'animation de fin de grandissement de la photo.")]
    public Animator cadrePhotoAnimator;
    public string fileName;

    [Tooltip("La Transform qui contiendra les motifs pendant l'édition.")]
    public Transform motifsParent;
    bool printPhoto = false;
    public bool showGizmos = false;



    public static PanelPhotoButtons instance;
    EpreuvePhoto epreuvePhoto;
    CameraPhotoScript camPhoto;


    #endregion



    #region Mono


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


    //On cache ce panel au retour sur la scène ppale
    private void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            gameObject.SetActive(false);

        }
    }



    private void Start()
    {
        epreuvePhoto = (EpreuvePhoto)Epreuve.instance;
        camPhoto = FindObjectOfType<CameraPhotoScript>();

    }








    //On remet le canvas en screen space camera avant le fondu en noir vers la scène ppale
    private void OnDisable()
    {
        StartCoroutine(OnDisableCo());
    }

    private IEnumerator OnDisableCo()
    {
        //On arrête l'occlusion pour éviter les artefacts
        FindObjectOfType<AROcclusionManager>().enabled = false;
        epreuvePhoto.session.Reset();

        ApplicationManager.SetResoltuion(1024, 768);
        yield return null;

        uiProtoCanvas.renderMode = RenderMode.ScreenSpaceCamera;

        Destroy(photoImg.sprite); //Pour gagner de la place
        Destroy(photoTex); //Pour gagner de la place


        ApplicationManager.CollectGarbage();
    }

    //initiailisation des paramètres
    private void OnEnable()
    {
        StartCoroutine(OnEnableCo());
    }


    private IEnumerator OnEnableCo()
    {

        ApplicationManager.SetResoltuion(2048, 1536);
        yield return null;



        StartCoroutine(Overlay());

        //On désactive les autres boutons du canvas
        for (int i = 0; i < buttonsToDisableOnEnable.Length; i++)
        {
            buttonsToDisableOnEnable[i].SetActive(false);
        }

        //On active les boutons liés au panel de photo
        Button[] b = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < b.Length; i++)
        {
            b[i].interactable = true;
        }



        //On vide les motifs
        ClearMotifList();
        addedMotifs = new List<GameObject>();


        //On initialise les variables et les images des sous-panels
        homeButton.gameObject.SetActive(false);
        fadePhotoImg.gameObject.SetActive(false);
        reticleCircle.enabled = false;
        reticleText.enabled = false;
        reticleBtn.gameObject.SetActive(true);
        reticleProfile.SetActive(true);


        scrollBarMotifs.value = 0f;

        panelLoading.SetActive(false);
        panelPrintDone.SetActive(false);

        countdownHasStarted = false;
        timer = 0f;

        ClosePanel(1);
        OpenPanel(0);

    }









    //On remet le canvas en screen space camera après le fondu en noir
    private IEnumerator Overlay()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        yield return wait;

        uiProtoCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

    }


    // Update is called once per frame
    void LateUpdate()
    {

        //Si on a appuyé sur le bouton Photo, on lance le timer avant la capture
        if (countdownHasStarted)
        {
            if(timer < countdown)
            {

                timer += Time.deltaTime;
                reticleCircle.fillAmount = 1f-(timer / countdown);
                reticleText.text = (countdown - Mathf.RoundToInt(timer)).ToString();
            }
            else
            {
                timer = 0f;
                countdownHasStarted = false;

                reticleCircle.enabled = false;
                reticleText.enabled = false;


                AudioManager.instance.Stop(epreuvePhoto.loadingPhotoClip);
                AudioManager.instance.Play(epreuvePhoto.photoClip);
                StartCoroutine(TakePhoto());

                ScreenTransitionImageEffect.instance.FadeImg(fadePhotoImg, .4f, Color.white);
                ScreenTransitionImageEffect.instance.onImageFadeInMiddleOfTransition += OpenPanel;
                ScreenTransitionImageEffect.instance.onImageFadeInMiddleOfTransition += epreuvePhoto.HideCharacters;



            }
        }


        //Si on a appuyé sur le bouton Photo, on lance la coroutine d'impression
        if (printPhoto)
        {
            printPhoto = false;
            StartCoroutine(PrintPhoto());


            //On libère de la place en mémoire.
            //On l'appelle juste avant la prise de photo car c'est l'endroit
            //le moins à risque niveau baisse de performances
            ApplicationManager.CollectGarbage();

        }
    }



    #endregion





    #region Buttons




    //Appelée par le bouton TakePhoto
    public void TakePhotoBtn()
    {



        EpreuvePhoto e = (EpreuvePhoto)Epreuve.instance;
        if (e.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        AudioManager.instance.Play(epreuvePhoto.startPhotoClip);
        AudioManager.instance.Play(epreuvePhoto.loadingPhotoClip);

        countdownHasStarted = true;
        reticleProfile.SetActive(false);
        reticleBtn.gameObject.SetActive(false);
        reticleCircle.enabled = true;
        reticleText.enabled = true;


    }

    //Pour reprendre la photo
    public void RetryTakePhotoBtn()
    {
        EpreuvePhoto e = (EpreuvePhoto)Epreuve.instance;
        if (e.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        Destroy(photoImg.sprite); //Pour gagner de la place
        Destroy(photoTex); //Pour gagner de la place
        ClearMotifList();


        //On affiche le panneau du réticule
        reticleBtn.gameObject.SetActive(true);
        reticleProfile.SetActive(true);
        OpenPanel(0);
        ClosePanel(1);


        //On réactive persos pour la prochaine photo
        //en on relance le dialogue
        epreuvePhoto.characters.SetActive(true);
        epreuvePhoto.EnableArOcclusion(true);

        //On arrête la détection AR
        epreuvePhoto.EnableArOcclusion(true);

        epreuvePhoto.PlayRecommencerDialogue();



        //On libère de la place en mémoire.
        //On l'appelle juste avant la prise de photo car c'est l'endroit
        //le moins à risque niveau baisse de performances
        ApplicationManager.CollectGarbage();

    }


    //Pour imprimer la photo
    public void PrintPhotoBtn()
    {
        
        if (epreuvePhoto.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        printPhoto = true;



    }





    public void OpenPanel(int index)
    {
        panels[index].SetActive(true);
        OnPanelOpened(index);

    }

    private void OnPanelOpened(int index)
    {
        switch (index)
        {
            case 0:
                reticleBtn.enabled = true;
                break;
            case 1:
                //Enlever tous les motifs enregistrés
                break;
        }
    }

    public void ClosePanel(int index)
    {
        panels[index].SetActive(false);
    }


    #endregion




    #region Motifs


    //Ces 4 fonctions gèrent l'ajout et le retrait de motifs

    public void AddMotifToList(GameObject motif)
    {

        EpreuvePhoto e = (EpreuvePhoto)Epreuve.instance;
        if (e.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        addedMotifs.Add(motif);
    }

    public void RemoveMotifFromList(GameObject motif)
    {
        EpreuvePhoto e = (EpreuvePhoto)Epreuve.instance;
        if (e.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        addedMotifs.Remove(motif);
        motifCount--;


        Destroy(motif);
    }

    public void ClearMotifList()
    {
        epreuvePhoto = (EpreuvePhoto)Epreuve.instance;
        if (epreuvePhoto.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        foreach (GameObject go in addedMotifs)
        {
            Destroy(go);
        }

        addedMotifs.Clear();
        motifCount = 0;


        ApplicationManager.CollectGarbage();
    }



    //Gèrent le placement des motifs dans le cadre photo

    public bool MotifContains(Vector2 center)
    {
        bool contains = false;

        foreach (GameObject go in addedMotifs)
        {
            RectTransform rt = go.GetComponent<RectTransform>();

            if (rt.GetWorldSapceRect().Contains(center))
            {
                contains = true;
                break;
            }

        }

        return contains;
    }


    public  bool ContainingMotifIsOutOfRect(Rect reducedRect, Vector2 center)
    {
        bool outOfRect = false;

        if (MotifContains(center))
        {
            outOfRect = !reducedRect.Contains(center);
        }
        return outOfRect;
    }



    #endregion




    #region Photo

    private IEnumerator TakePhoto()
    {
        //On écrase le chemin de la précédente photo pour y enregistrer la nouvelle
        path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
            File.Delete(path);



        yield return null;
        camPhoto.TakeScreenshot(path);

        yield return null;

        yield return null;





        //On arrête la détection AR
        epreuvePhoto.EnableArOcclusion(false);



        photoTex = GetPhotoTextureFromDataPath(path);
        photoImg.sprite = Sprite.Create(photoTex, new Rect(0.0f, 0.0f, photoTex.width, photoTex.height), new Vector2(.5f, .5f), 100f);
        photoImg.sprite.name = fileName;


        WaitForSeconds wait = new WaitForSeconds(1f);
        yield return wait;
        epreuvePhoto.PlayMotifDialogue();


    }

    private Texture2D GetPhotoTextureFromDataPath(string path)
    {
        if (File.Exists(path))
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(File.ReadAllBytes(path));


            return tex;
        }

        else
        {
            return null;
        }

    }







    private IEnumerator PrintPhoto()
    {



        //On désactive les autres boutons
        Button[] b = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < b.Length; i++)
        {
            b[i].interactable = false;
        }



        //On n'imprime pas la photo tant que le cadre n'a pas été mis  en plein écran
        cadrePhotoAnimator.Play("final");

        while (cadrePhotoAnimator.GetCurrentAnimatorStateInfo(0).IsName("final"))
        {
            yield return null;
        }


        AudioManager.instance.Play(epreuvePhoto.startPhotoClip);


        WaitForSeconds wait = new WaitForSeconds(2f);
        yield return wait;






        //On utilise une méthode différente en fonction de la machine sur laquelle on teste
#if UNITY_EDITOR || UNITY_STANDALONE

        //On reprend une capture d'écran de la photo une fois rescalée et repositionnée pour prendre tout l'écran.
        //On la sauvegarde ailleurs et on l'imprime.


        motifsParent.transform.SetParent(photoImg.transform);
        camPhoto.TakeScreenshot(path);

        yield return StartCoroutine(PrintSender.instance.PrintImage(path, fileName, 1));
#elif UNITY_IOS
        Texture2D photo = ScreenCapture.CaptureScreenshotAsTexture();
        PrintSender.instance.PrintImage(photo);
#endif



        //Une fois la photo imprimée, on termine l'épreuve et on reivent à la scène ppale où se jouera le dialogue de fin

        panelLoading.SetActive(true);

        AudioManager.instance.Play(epreuvePhoto.loadingPhotoClip);


        wait = new WaitForSeconds(1.5f);
        yield return wait;

        AudioManager.instance.Stop(epreuvePhoto.loadingPhotoClip);
        AudioManager.instance.Play(epreuvePhoto.donePhotoClip);

        panelLoading.SetActive(false);
        panelPrintDone.SetActive(true);


        wait = new WaitForSeconds(2f);
        yield return wait;

        uiProtoCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        epreuvePhoto.EndEpreuve();
        ScreenTransitionImageEffect.instance.FadeToScene(1);
    }


    #endregion
}
