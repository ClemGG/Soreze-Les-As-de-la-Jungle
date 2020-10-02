using UnityEngine;

//Ce script est appelé quand le joueur reprend une partie en cours
//pour bloquer la cinématique d'intro et éviter de la rejouer
//si le joueur l'a déjà vu.

public class AnimIntroBlocker : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        FindObjectOfType<AnimIntro>().bypassIntro = true;

        if(level > 0)
        Destroy(gameObject);
    }
}
