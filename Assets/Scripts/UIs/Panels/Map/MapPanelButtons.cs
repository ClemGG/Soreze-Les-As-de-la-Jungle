using UnityEngine;

public class MapPanelButtons : MonoBehaviour
{
    public Location[] locations;
    public Sprite normalImg, checkImg;

    public GameObject[] oeuvrePreviews;    //Affichée quand on clique sur un des marqueurs de la carte
    int curPreviewID = -1;

    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]

    public AudioClip openMapClip;


    public static MapPanelButtons instance;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }


    public void OnEnable()
    {
        for (int i = 0; i < oeuvrePreviews.Length; i++)
        {
            oeuvrePreviews[i].SetActive(false);
        }
        AudioManager.instance.Play(openMapClip);



        //Quand le panel de la map s'active, on récupère tous les points indiquant la position d'une oeuvre
        //et on regarde si l'épreuve correspondante a été terminée.
        //Si oui, on affiche le symbole Valider

        for (int i = 0; i < locations.Length; i++)
        {
            Location l = locations[i];

            if (PlayerPrefs.HasKey($"EpreuveVictory{l.ID}"))
            {
                l.unlocked = PlayerPrefs.GetInt($"EpreuveVictory{l.ID}", 0) == 1 ? true : false;
                l.img.sprite = l.unlocked ? checkImg : normalImg;
            }
            else
            {
                l.unlocked = false;
                l.img.sprite = normalImg;
            }
        }
    }


    //Appelée quand on clique sur un des marqueurs de la carte
    public void DisplayPreview(int ID)
    {
        if(curPreviewID == ID)
        {
            oeuvrePreviews[curPreviewID].SetActive(!oeuvrePreviews[curPreviewID].activeSelf);
        }
        else
        {
            curPreviewID = ID;

            for (int i = 0; i < oeuvrePreviews.Length; i++)
            {
                oeuvrePreviews[i].SetActive(i == ID);
            }

        }

    }

    public void HidePreview()
    {
        oeuvrePreviews[curPreviewID].SetActive(false);
    }
}

