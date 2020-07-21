using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SceneFader : MonoBehaviour {


    public bool fadeAtStart = false;
    public Image fadeImg;

    [Tooltip("Plus la valeur est haute, plus le fondu sera rapide, et inversement plus la valeur est basse.")]
    public float fadeAlphaSpeed = 1f;


    public AnimationCurve fadeCurve;


    public delegate void OnPhotoFinished(int index);
    public OnPhotoFinished onPhotoFinished;
    public OnPhotoFinished onPhotoInMiddleOfTransition;

    public delegate void OnSceneTransition();
    public OnSceneTransition onMiddleOfSceneTransition;


    public static SceneFader instance;

    private void Awake()
    {
        if (instance != null)
        {
            print("More than one SceneFader in scene !");
            return;
        }

        instance = this;
        
    }





    private void Start()
    {
        if (fadeAtStart)
        {

            if (fadeImg.gameObject.activeSelf == false)
            {
                fadeImg.gameObject.SetActive(true);
            }

            StartCoroutine(FadeIn());
        }
    }





    public static int CurrentLevelIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public static string CurrentLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }





    /// <summary>
    /// Permet de réaliser un fondu entre les scènes
    /// </summary>
    public void FadeToScene(int sceneIndex)
    {
        StartCoroutine(FadeOut(sceneIndex));
    }

 


    /// <summary>
    /// Permet de réaliser un fondu avant de quitter le jeu.
    /// </summary>
    public void FadeToQuitScene()
    {
        StartCoroutine(FadeQuit());
    }




    public void FadeImg(Image fadePhotoImg, float fadeDuration, Color color)
    {

        StartCoroutine(FadeImgInOut(fadePhotoImg, fadeDuration, color));
        
    }

    private IEnumerator FadeImgInOut(Image fadePhotoImg, float fadeDuration, Color color)
    {
        fadePhotoImg.gameObject.SetActive(true);
        float t = 0f;

        Time.timeScale = 1f / fadeDuration;

        while (t < 1f)
        {
            t += Time.deltaTime;
            float a = fadeCurve.Evaluate(t);
            fadePhotoImg.color = new Color(color.r, color.g, color.b, a);
            yield return 0;
        }

        yield return new WaitForSeconds(.1f);
        onPhotoInMiddleOfTransition?.Invoke(1);

        t = 1f;


        while (t > 0f)
        {
            t -= Time.deltaTime;
            float a = fadeCurve.Evaluate(t);
            fadePhotoImg.color = new Color(color.r, color.g, color.b, a);
            yield return 0;
        }



        fadePhotoImg.gameObject.SetActive(false);
        Time.timeScale = 1f;

        onPhotoFinished?.Invoke(1);
    }













    /// <summary>
    /// Diminue l'alpha du fondu pour faire apparaître la scène
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeIn()
    {

        float t = 1f;


        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        fadeImg.gameObject.SetActive(false);

        

    }








    /// <summary>
    /// Augmente l'alpha du fondu pour faire disparaître la scène
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOut(int sceneIndex)
    {
        fadeImg.gameObject.SetActive(true);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        onMiddleOfSceneTransition?.Invoke();

        SceneManager.LoadScene(sceneIndex);
    }







    /// <summary>
    /// Augmente l'alpha du fondu pour faire disparaître la scène et quitter le jeu.
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeQuit()
    {
        fadeImg.gameObject.SetActive(true);
        float t = 0f;



        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }


        Application.Quit();
    }
}
