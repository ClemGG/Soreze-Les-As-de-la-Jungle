using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    public SettingsPanelButtons settingsPanel;
    public GameObject resumeButton, animIntroBlocker, resetBagOnStartup;


    public static MainMenuButtons instance;

    //Singleton
    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }

        instance = this;


    }



    private void Start()
    {
        //Si on a pas de partie enregistrée, on cache le bouton reprendre
        //pour ne laisser que le bouton nouvelle partie
        animIntroBlocker = FindObjectOfType<AnimIntroBlocker>().gameObject;
        resetBagOnStartup = FindObjectOfType<ResetBagOnStartup>().gameObject;
        animIntroBlocker.SetActive(false);
        resetBagOnStartup.SetActive(false);

        if (!PlayerPrefs.HasKey("partie en cours"))
        {
            resumeButton.SetActive(false);
        }


        //On active le panel des paramètres et on change la langue avant de le refermer

        settingsPanel.gameObject.SetActive(true);

        if (PlayerPrefs.HasKey("langue"))
        {
            settingsPanel.ChangerLangue(PlayerPrefs.GetString("langue"));
        }
        else
        {
            settingsPanel.ChangerLangue("fr");
        }


        settingsPanel.gameObject.SetActive(false);
    }


    //Appelée par le bouton Play
    public void PlayButton(bool eraseSave)
    {
        //Si on crée une nouvelle partie, on efface la sauvegarde.
        //On recrée la clé de la langue pour éviter de la perdre
        if (eraseSave)
        {
            string curLangue = PlayerPrefs.GetString("langue");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("langue", curLangue);
        }


        //On met la langue par défaut sur FR si le joueur ne l'a pas déjà mis

        if (!PlayerPrefs.HasKey("langue"))
            PlayerPrefs.SetString("langue", "fr");


        //On active les sons
        settingsPanel.gameObject.SetActive(true);
        settingsPanel.ToggleAllSounds(true);
        settingsPanel.gameObject.SetActive(false);


        if (eraseSave)
        {
            //C'est une nouvelle partie, on supprime les playerPrefs
            //et on cache les oeuvres du sac
            if(resetBagOnStartup) resetBagOnStartup.SetActive(true);
            Destroy(animIntroBlocker);

        }
        else
        {
            //Comme on reprend la partie, on ne veut pas activer la cinématique d'intro,
            //donc on envoie un objet la désactiver
            if(animIntroBlocker) animIntroBlocker.SetActive(true);
            Destroy(resetBagOnStartup);
        }



        //On lance la scène ppale
        ScreenTransitionImageEffect.instance.FadeToScene(1);
    }





}
