using UnityEngine;

public class SettingsPanelButtons : MonoBehaviour
{

    public LanguageButtons[] languageButtons;
    public GameObject buttonToggleOn, buttonToggleOff;


    //A l'activation du panel des paramètres, on initialise les sprites pour indiquer au joueur son dernier choix
    private void OnEnable()
    {
        for (int i = 0; i < languageButtons.Length; i++)
        {
            languageButtons[i].ChangeIcon();
        }


        if (buttonToggleOn) buttonToggleOn.SetActive(PlayerPrefs.GetInt("bruitages", 1) == 1);
        if (buttonToggleOff) buttonToggleOff.SetActive(PlayerPrefs.GetInt("bruitages", 1) == 0);
    }



    //Pour les sons, on récupère tous les sons enregistrés dans l'AudioManager et on inverse leur état

    public void ToggleAudio(bool active)
    {
        if (buttonToggleOn) buttonToggleOn.SetActive(active);
        if (buttonToggleOff) buttonToggleOff.SetActive(!active);

        ToggleAllSounds(active);
    }

    public void ToggleAllSounds(bool active)
    {
        for (int i = 0; i < AudioManager.instance.sons.Length; i++)
        {
            if (active)
                AudioManager.instance.sons[i].clip.LoadAudioData();
            else
                AudioManager.instance.sons[i].clip.UnloadAudioData();


            AudioManager.instance.sons[i].source.gameObject.SetActive(active);
        }
        PlayerPrefs.SetInt("bruitages", active ? 1 : 0);
        PlayerPrefs.SetInt("musique", active ? 1 : 0);
    }


    public void ChangerLangue(string fileLanguage)
    {
        //On appelle le LocalizationManager pour changer tous les textes instantanément
        LocalizationManager.instance.LoadLocalizedText(fileLanguage);

        //On garde en mémoire la nouvelle langue
        PlayerPrefs.SetString("langue", fileLanguage);

        //On change les UIs pour refléter le changement de langue
        OnEnable();
    }
}
