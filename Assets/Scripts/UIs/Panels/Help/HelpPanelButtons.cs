using System.Collections;
using UnityEngine;


//utilisé pendant les épreuves pour afficher le bouton du caméléon et les aides.

public class HelpPanelButtons : MonoBehaviour
{
    public static HelpPanelButtons instance;
    public GameObject cameleon;
    Animator cameleonAnim;
    Epreuve curEpreuve;

    bool isHelpActive;

    //Singleton
    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
        cameleonAnim = cameleon.GetComponent<Animator>();
    }


    //Au début de chaque niveau, on cache le caméléon
    private void OnLevelWasLoaded(int level)
    {

        if (MainSceneButtons.instance.introDone)
            StartCoroutine(DisableCameleon(level));

        curEpreuve = FindObjectOfType<Epreuve>();
        isHelpActive = false;
        
    }

    private IEnumerator DisableCameleon(int level)
    {
        yield return new WaitForSeconds(1f);

        //Si on est dans la scène ppale, celle des échafaudages ou de la photo, on cache l'aide
        if (level == 1 || level > 7)
        {

            cameleon.SetActive(false);
        }
        else
        {
            //Sinon, on l'active et on joue son idle
            cameleon.SetActive(true);
            cameleonAnim.Play("a_cameleon_idle_en_haut");
        }

    }


    public void ShowHelp()
    {
        if (!isHelpActive)
        {
            cameleonAnim.Play("a_cameleon_descente");
            isHelpActive = true;
        }
    }


    //Appelée par le btn du caméléon
    public void GetEpreuveHelp()
    {
        if (curEpreuve)
        {
            if (!curEpreuve.EpreuveFinished)
            {
                curEpreuve.SendHelp();
                cameleonAnim.Play("a_cameleon_remontee");
                isHelpActive = false;
            }
        }
    }


    //Pour permettre au joueur d'appeler lui-même l'aide si besoin
    public void CallHelp()
    {
        if (curEpreuve)
        {
            if (!curEpreuve.EpreuveFinished)
            {
                curEpreuve.SetHelpTimerToZero();
            }
        }
    }
}
