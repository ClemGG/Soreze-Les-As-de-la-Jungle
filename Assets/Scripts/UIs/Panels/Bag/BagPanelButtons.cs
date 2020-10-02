using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BagPanelButtons : MonoBehaviour
{
    #region Variables

    public GameObject panelBag;

    [Tooltip("les chevalets du panel Bag")]
    public GameObject[] oeuvresIcons;


    [Tooltip("les ScriptableObjects contenant les infos sur chaque oeuvre.")]
    Oeuvre[] oeuvres;


    [Space(10)]

    public GameObject panelDescription;
    public GameObject panelChevalets;

    [Space(10)]

    //public Image oeuvreImg;
    [Tooltip("les images à activer en haut à gauche du panel de description")]
    public Image[] oeuvreImages;

    [Tooltip("les tests descriptifs")]
    public TextMeshProUGUI titreOeuvre, referenceOeuvre, questionG, questionD, descriptionG, descriptionD;

    [Tooltip("Pour cacher le panel de description")]
    public CanvasGroup oeuvreDescriptionAlpha;


    public static BagPanelButtons instance;

    #endregion



    #region Mono

    //Singleton
    protected void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }


    //Au démarrage, on cache le UI du sac et on réinitialise les scriptable objects des oeuvres
    private void Start()
    {
        panelBag.SetActive(false);
        DisplayUnlockedOeuvresAndCollectibles();
        DisplayOeuvreDescription(-1);

    }


    //Quand on active le panel à nouveau, on affiche les chevalets et leurs descriptions si l'épreuve correspondante
    //a été terminée
    public void OnEnable()
    {


        panelBag.SetActive(true);
        panelChevalets.SetActive(true);
        panelDescription.SetActive(false);
        DisplayUnlockedOeuvresAndCollectibles();
        DisplayOeuvreDescription(-1);
        

    }


    #endregion



    #region Bag

    public void DisplayUnlockedOeuvresAndCollectibles()
    {
        oeuvres = Resources.LoadAll<Oeuvre>($"Bag/Oeuvres/{PlayerPrefs.GetString("langue", "fr").ToUpper()}");

        //Pour afficher les oeuvres quelque soit la langue
        for (int i = 0; i < oeuvres.Length; i++)
        {
            oeuvres[i].unlocked = PlayerPrefs.GetInt("Oeuvre" + oeuvres[i].ID.ToString(), 0) == 1;
        }

        panelChevalets.SetActive(true);
        panelDescription.SetActive(false);



        //Pour chacune des oeuvres, on regarde si elle a été débloquée.
        //Si oui, on affiche les changements sur le chevalet et le panel descriptif
        //Sinon, on affiche le "?" pour indiquer au joueur qu'il n'a pas terminé l'épreuve
        for (int i = 0; i < oeuvres.Length; i++)
        {
            GameObject go = oeuvresIcons[oeuvres[i].ID];
            //print(oeuvres[i].name + ",  " + oeuvres[i].unlocked);

            if (go)
            {
                Image oeuvreImg = go.transform.GetChild(0).GetComponent<Image>();
                Image oeuvreImgNull = go.transform.GetChild(1).GetComponent<Image>();
                TextMeshProUGUI oeuvreTitle = go.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI oeuvreMaterials = go.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                Button btn = go.transform.GetChild(4).GetComponent<Button>();


                oeuvreImg.enabled = oeuvres[i].unlocked ? true : false;

                oeuvreImgNull.enabled = oeuvres[i].unlocked ? false : true;
                oeuvreTitle.text = oeuvres[i].unlocked ? oeuvres[i].title : null;
                oeuvreMaterials.text = oeuvres[i].unlocked ? oeuvres[i].materials : null;

                btn.gameObject.SetActive(oeuvres[i].unlocked);

            }

        }

    }

    public void ChangeOeuvreLockState(int epreuveID)
    {
        PlayerPrefs.SetInt($"Oeuvre{epreuveID}", 1);
    }


    //Pour cacher à nouveau toutes les oeuvres
    public void ResetAllOeuvresAndCollectibles()
    {
        for (int i = 0; i < oeuvres.Length; i++)
        {
            oeuvres[i].unlocked = false;
            PlayerPrefs.SetInt($"Oeuvre{i}", 0);
        }

        DisplayUnlockedOeuvresAndCollectibles();
    }



    //Quand on affiche le panel descriptif, on assigne aux images et textes l'oeuvre correspondante
    public void DisplayOeuvreDescription(int index)
    {

        if (index < 0 || index >= 0 && !oeuvres[index])
        {

            for (int i = 0; i < oeuvreImages.Length; i++)
            {
                oeuvreImages[i].gameObject.SetActive(false);
            }

            oeuvreDescriptionAlpha.alpha = 0f;


            //titreOeuvre.text = null;
            referenceOeuvre.text = null;
            questionG.text = null;
            questionD.text = null;
            descriptionG.text = null;
            descriptionD.text = null;
        }
        else
        {

            panelChevalets.SetActive(false);
            panelDescription.SetActive(true);

            //oeuvreImages[index].sprite = oeuvres[index].img;
            for (int i = 0; i < oeuvreImages.Length; i++)
            {
                oeuvreImages[i].gameObject.SetActive(i == index);
            }

            titreOeuvre.text = string.Format("<size=75>{0} </size>\n{1} {2}",
                                                     oeuvres[index].title,
                                                     oeuvres[index].author,
                                                     oeuvres[index].date);


            referenceOeuvre.text = oeuvres[index].reference;
            questionG.text = oeuvres[index].questionG;
            questionD.text = oeuvres[index].questionD;
            descriptionG.text = oeuvres[index].descriptionG;
            descriptionD.text = oeuvres[index].descriptionD;

            oeuvreDescriptionAlpha.alpha = 1f;
        }

    }

    #endregion

}

