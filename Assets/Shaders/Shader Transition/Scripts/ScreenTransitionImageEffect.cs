// This code is related to an answer I provided in the Unity forums at:
// http://forum.unity3d.com/threads/circular-fade-in-out-shader.344816/

using UnityEngine;
using System.Collections;

using static Clement.Utilities.SceneManaging;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Screen Transition")]
public class ScreenTransitionImageEffect : MonoBehaviour
{


    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public static ScreenTransitionImageEffect instance;
    public Shader shader;
    public Camera camUsed;

    //[Tooltip("Les tags que doivent porter les canvas pour être masqués par l'effet de transition")]
    //public string[] canvasTags; //Les tags que doivent porter les canvas pour être masqués par l'effet de transition




    [Space(10)]
    [Header("Transition Effect : ")]
    [Space(10)]


    [Tooltip("Plus la valeur est haute, plus le fondu sera rapide, et inversement plus la valeur est basse.")]
    public float fadeAlphaSpeed = 1f;
    public AnimationCurve fadeCurve;
    [HideInInspector] public bool isTransitioning = false;

    [Space(10)]

    [Range(0,1.0f)]
    public float maskValue = .5f;
    public Color maskColor = Color.black;
    public Texture2D maskTexture;
    public bool maskInvert;

    private Material m_Material;
    private bool m_maskInvert;


    public delegate void OnImageFadeFinished(int index);
    public OnImageFadeFinished onImageFadeFinished;
    public OnImageFadeFinished onImageFadeInMiddleOfTransition;


    public delegate void OnSceneTransition();
    public OnSceneTransition onBeginningOfSceneTransition;
    public OnSceneTransition onMiddleOfSceneTransition;
    public OnSceneTransition onEndOfSceneTransition;



#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            isTransitioning = false;
        }
    }

#endif


    #region MonoBehaviour

    private void OnLevelWasLoaded(int level)
    {
        Canvas[] allCanvasesInScene = FindAllObjectsInSceneOfType<Canvas>().ToArray();

        for (int i = 0; i < allCanvasesInScene.Length; i++)
        {
            //for (int j = 0; j < canvasTags.Length; j++)
            //{
            //    if (allCanvasesInScene[i].tag == canvasTags[j])
            //    {
            //print(allCanvasesInScene[i].name);

            if (allCanvasesInScene[i].renderMode != RenderMode.WorldSpace)
            {
                allCanvasesInScene[i].renderMode = RenderMode.ScreenSpaceCamera;
                allCanvasesInScene[i].worldCamera = camUsed;
                allCanvasesInScene[i].planeDistance = 1f;
                //allCanvasesInScene[i].sortingOrder = 1;
            }
            //break;
            //    }
            //}

        }
    }


    private void Awake()
    {
        if (instance != null)
        {
            print("More than one ScreenTransitionImageEffect in scene !");
            return;
        }

        instance = this;

    }


    IEnumerator Start()
    {
        #region Shader

        // Disable if we don't support image effects
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
        }
        else
        {
            shader = Shader.Find("Hidden/ScreenTransitionImageEffect");

            // Disable the image effect if the shader can't
            // run on the users graphics card
            if (shader == null || !shader.isSupported)
                enabled = false;
        }
        #endregion

        isTransitioning = false;
        //OnLevelWasLoaded(0);
        maskValue = 1f;
        yield return new WaitForSeconds(1f);

        //Faire gaffe à ce que IsTransitioning soit à false dans l'inspector
        if (!isTransitioning)
            StartCoroutine(FadeIn());

    }


    #endregion






    #region Scene Fader






    public static int CurrentLevelIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public static string CurrentLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static int GetSceneIndexByName(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        int desiredIndex = 0;

        for (int i = 0; i < sceneCount; i++)
        {
            string curSceneName = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
            //print(curSceneName);

            if (curSceneName.Trim().Equals(sceneName.Trim()))
            {
                desiredIndex = i;
            }
        }

        return desiredIndex;
    }






    /// <summary>
    /// Permet de réaliser un fondu entre les scènes
    /// </summary>
    public void FadeToScene(int sceneIndex)
    {
        OnLevelWasLoaded(-1);

        if(!isTransitioning)
            StartCoroutine(FadeOut(sceneIndex));
    }




    /// <summary>
    /// Permet de réaliser un fondu avant de quitter le jeu.
    /// </summary>
    public void FadeToQuitScene()
    {
        if(!isTransitioning)
            StartCoroutine(FadeQuit());
    }




    public void FadeImg(Image fadePhotoImg, float fadeDuration, Color color)
    {

        if(!isTransitioning)
            StartCoroutine(FadeImgInOut(fadePhotoImg, fadeDuration, color));

    }

    private IEnumerator FadeImgInOut(Image fadePhotoImg, float fadeDuration, Color color)
    {
        isTransitioning = true;

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
        onImageFadeInMiddleOfTransition?.Invoke(1);

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
        isTransitioning = false;

        onImageFadeFinished?.Invoke(1);
    }













    /// <summary>
    /// Diminue l'alpha du fondu pour faire apparaître la scène
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeIn()
    {
        isTransitioning = true;

        float t = 1f;
        maskValue = t;


        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * fadeAlphaSpeed;
            maskValue = fadeCurve.Evaluate(t);
            yield return 0;
        }

        isTransitioning = false;

    }








    /// <summary>
    /// Augmente l'alpha du fondu pour faire disparaître la scène
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOut(int sceneIndex)
    {
        isTransitioning = true;

        float t = 0f;
        maskValue = t;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeAlphaSpeed;
            maskValue = fadeCurve.Evaluate(t);
            yield return 0;
        }

        onMiddleOfSceneTransition?.Invoke();
        isTransitioning = false;

        SceneManager.LoadScene(sceneIndex);
    }







    /// <summary>
    /// Augmente l'alpha du fondu pour faire disparaître la scène et quitter le jeu.
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeQuit()
    {
        isTransitioning = true;

        float t = 0f;
        maskValue = t;



        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeAlphaSpeed;
            maskValue = fadeCurve.Evaluate(t);
            yield return 0;
        }

        isTransitioning = false;

        Application.Quit();
    }

    #endregion









    #region Shader
    Material material
    {
        get
        {
            if (m_Material == null)
            {
                m_Material = new Material(shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
        }
    }

    void OnDisable()
    {
        if (m_Material)
        {
            DestroyImmediate(m_Material);
        }
        maskValue = 0f;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!enabled)
        {
            Graphics.Blit(source, destination);
            return;
        }

        material.SetColor("_MaskColor", maskColor);
        material.SetFloat("_MaskValue", maskValue);
        material.SetTexture("_MainTex", source);
        material.SetTexture("_MaskTex", maskTexture);

        if (material.IsKeywordEnabled("INVERT_MASK") != maskInvert)
        {
            if (maskInvert)
                material.EnableKeyword("INVERT_MASK");
            else
                material.DisableKeyword("INVERT_MASK");
        }

        Graphics.Blit(source, destination, material);
    }

    #endregion
}
