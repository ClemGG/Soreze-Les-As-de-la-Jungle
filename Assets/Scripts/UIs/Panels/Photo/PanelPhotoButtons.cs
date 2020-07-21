using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelPhotoButtons : MonoBehaviour
{
    public Canvas uiProtoCanvas;

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

    [Tooltip("Si on veut afficher le nb de motifs à l'écran.")]
    public Text motifsOnScreenText;


    [Space(10)]
    [Header("Print : ")]
    [Space(10)]

    //public InputField emailField;
    [Tooltip("La photo en elle-même, qui contiendra les motifs à la fin de l'édition.")]
    public Image photoImg;

    public TextMeshProUGUI nbCopiesText;
    public Button btnPlus, btnMoins;
    int nbCopies = 1;
    Vector2Int nbCopiesLimit = new Vector2Int(1, 5);

    [Space(10)]

    public GameObject panelLoading, panelPrintDone;

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


    private void Start()
    {
        epreuvePhoto = (EpreuvePhoto)Epreuve.instance;
        camPhoto = FindObjectOfType<CameraPhotoScript>();
    }

    //On remet le canvas en screen space camera avant le fondu en noir vers la scène ppale
    private void OnDisable()
    {

        uiProtoCanvas.renderMode = RenderMode.ScreenSpaceCamera;
    }

    //initiailisation des paramètres
    private void OnEnable()
    {
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
        fadePhotoImg.gameObject.SetActive(false);
        reticleCircle.enabled = false;
        reticleText.enabled = false;
        reticleBtn.gameObject.SetActive(true);
        reticleProfile.SetActive(true);


        scrollBarMotifs.value = 0f;

        nbCopies = 1;
        nbCopiesText.text = "1";
        btnPlus.interactable = nbCopies < 5;
        btnMoins.interactable = nbCopies > 1;

        panelLoading.SetActive(false);
        panelPrintDone.SetActive(false);

        countdownHasStarted = false;
        timer = 0f;

        OpenPanel(0);
        ClosePanel(1);
        ClosePanel(2);

        UpdateMotifUI();
    }

    //On remet le canvas en screen space camera après le fondu en noir
    private IEnumerator Overlay()
    {
        yield return new WaitForSeconds(5f);
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


                nbCopies = nbCopiesLimit.x;
                nbCopiesText.text = nbCopies.ToString();

                AudioManager.instance.Stop(epreuvePhoto.loadingPhotoClip);
                AudioManager.instance.Play(epreuvePhoto.photoClip);
                StartCoroutine(TakePhoto());

                //SceneFader.instance.FadeImg(fadePhotoImg, .4f, Color.white);
                //SceneFader.instance.onPhotoInMiddleOfTransition += OpenPanel;
                ScreenTransitionImageEffect.instance.FadeImg(fadePhotoImg, .4f, Color.white);
                ScreenTransitionImageEffect.instance.onImageFadeInMiddleOfTransition += OpenPanel;
                //SceneFader.instance.onPhotoFinished += OpenPanel;
            }
        }


        //Si on a appuyé sur le bouton Photo, on lance la coroutine d'impression
        if (printPhoto)
        {
            printPhoto = false;
            StartCoroutine(PrintPhoto());
        }
    }


    //On cache ce panel au retour sur la scène ppale
    private void OnLevelWasLoaded(int level)
    {
        if(level == 1)
        {
            gameObject.SetActive(false);

        }
    }










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


        nbCopies = 1;
        nbCopiesText.text = "1";
        btnPlus.interactable = nbCopies < 5;
        btnMoins.interactable = nbCopies > 1;

    }

    //Pour reprendre la photo
    public void RetryTakePhotoBtn()
    {
        EpreuvePhoto e = (EpreuvePhoto)Epreuve.instance;
        if (e.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        reticleBtn.gameObject.SetActive(true);
        reticleProfile.SetActive(true);

        ClearMotifList();

        OpenPanel(0);
        ClosePanel(1);

        epreuvePhoto.PlayRecommencerDialogue();
    }


    //Pour imprimer la photo
    public void PrintPhotoBtn()
    {
        EpreuvePhoto e = (EpreuvePhoto)Epreuve.instance;
        if (e.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        printPhoto = true;
    }

    //Pour choisir le nb d'exemplaires
    public void SetNbCopiesBtn(bool increase)
    {
        EpreuvePhoto e = (EpreuvePhoto)Epreuve.instance;
        if (e.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        nbCopies += increase ? 1 : -1;
        nbCopies = Mathf.Clamp(nbCopies, nbCopiesLimit.x, nbCopiesLimit.y);
        nbCopiesText.text = nbCopies.ToString();

        btnPlus.interactable = nbCopies < 5;
        btnMoins.interactable = nbCopies > 1;
    }




    public void OpenPanel(int index)
    {
        panels[index].SetActive(true);
        OnPanelOpened(index);

        if(SceneFader.instance)
            SceneFader.instance.onPhotoInMiddleOfTransition -= OpenPanel;
        //SceneFader.instance.onPhotoFinished -= OpenPanel;
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
            //case 2:
            //    emailField.text = "";
            //    break;
        }
    }

    public void ClosePanel(int index)
    {
        panels[index].SetActive(false);
    }









    //Ces 4 fonctions gèrent l'ajout et le retrait de motifs

    public void AddMotifToList(GameObject motif)
    {
        EpreuvePhoto e = (EpreuvePhoto)Epreuve.instance;
        if (e.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        //if(motifCount < maxMotifs)
        //{
        //    addedMotifs.Add(motif);
        //    motifCount++;

        //    UpdateMotifUI();
        //}
        addedMotifs.Add(motif);
    }

    public void RemoveMotifFromList(GameObject motif)
    {
        EpreuvePhoto e = (EpreuvePhoto)Epreuve.instance;
        if (e.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        addedMotifs.Remove(motif);
        motifCount--;

        UpdateMotifUI();

        Destroy(motif);
    }

    public void ClearMotifList()
    {

        EpreuvePhoto e = (EpreuvePhoto)Epreuve.instance;
        if (e.EpreuveFinished || DialogueEpreuveSystem.instance.isPlaying)
            return;

        foreach (GameObject go in addedMotifs)
        {
            Destroy(go);
        }

        addedMotifs.Clear();
        motifCount = 0;
        UpdateMotifUI();
    }

    private void UpdateMotifUI()
    {
        motifsOnScreenText.text = $"{motifCount} / {maxMotifs}";
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







    private IEnumerator TakePhoto()
    {
        //On écrase le chemin de la précédente photo pour y enregistrer la nouvelle
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
            File.Delete(path);


        yield return new WaitForEndOfFrame();

        camPhoto.TakeScreenshot(path);


        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();


        //On l'affiche ensuite dans le cadre photo

        Texture2D photoTex = GetPhotoTextureFromDataPath(path);
        photoImg.sprite = Sprite.Create(photoTex, new Rect(0.0f, 0.0f, photoTex.width, photoTex.height), new Vector2(.5f, .5f), 100f);

        photoImg.sprite.name = fileName;


        yield return new WaitForSeconds(1f);
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
        //On n'imprime pas la photo tant que le cadre n'a pas été mis  en plein écran
        cadrePhotoAnimator.Play("final");

        while (cadrePhotoAnimator.GetCurrentAnimatorStateInfo(0).IsName("final"))
        {
            yield return null;
        }

        AudioManager.instance.Play(epreuvePhoto.startPhotoClip);

        yield return new WaitForSeconds(3f);
        yield return new WaitForEndOfFrame();

        //On reprend une capture d'écran de la photo une fois rescalée et repositionnée pour prendre tout l'écran.
        //On la sauvegarde ailleurs et on l'imprime.

//#if UNITY_EDITOR || UNITY_STANDALONE_WIN

        string path = GetPathOfRescaledImageWithMotifs();
        yield return new WaitForEndOfFrame();
//#endif

        //On désactive les autres boutons
        Button[] b = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < b.Length; i++)
        {
            b[i].interactable = false;
        }


        //On utilise une méthode différente en fonction de la machine sur laquelle on teste
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

        yield return StartCoroutine(PrintSender.instance.PrintImage(path, fileName, nbCopies));
#elif UNITY_IOS
        yield return StartCoroutine(PrintSender.instance.PrintImage(/*nbCopies*/));
#endif

        //Une fois la photo imprimée, on termine l'épreuve et on reivent à la scène ppale où se jouera le dialogue de fin

        panelLoading.SetActive(true);

        AudioManager.instance.Play(epreuvePhoto.loadingPhotoClip);

        yield return new WaitForSeconds(2f);

        AudioManager.instance.Stop(epreuvePhoto.loadingPhotoClip);
        AudioManager.instance.Play(epreuvePhoto.donePhotoClip);

        panelLoading.SetActive(false);
        panelPrintDone.SetActive(true);

        yield return new WaitForSeconds(2f);

        uiProtoCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        FindObjectOfType<EpreuvePhoto>().EndEpreuve();
        ScreenTransitionImageEffect.instance.FadeToScene(1);
    }


    private string GetPathOfRescaledImageWithMotifs()
    {
        motifsParent.transform.SetParent(photoImg.transform);

        //On prend le PersistentDataPath parce que c'est là qu'on doit enregistrer les photos pour pouvoir les récupérer après. Sur l'IPad elles seront synchronisées sur leur Cloud normalement, donc elles seront pas perdues
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ScreenCapture.CaptureScreenshot(path);
        
        return path;
    }
}
