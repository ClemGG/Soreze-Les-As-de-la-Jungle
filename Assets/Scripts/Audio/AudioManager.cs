using System;
using System.Collections.Generic;
using UnityEngine;


/* Concentre tous les sons joués pdt la partie
 * Pour l'utiliser depuis un autre script, il faut déclarer un Audioclip répertorié
 * dans la liste de sons et le passer (ou son nom) dans la fonction appelée
 */

public class AudioManager : MonoBehaviour
{

    public Son[] sons;


    public static AudioManager instance;

#if UNITY_EDITOR

    private void OnValidate()
    {
        for (int i = 0; i < sons.Length; i++)
        {
            sons[i].tag = sons[i].clip.name;

        }
    }

#endif




    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }



    // Start is called before the first frame update
    void Start()
    {
        AudioListener[] audioListeners = (AudioListener[])Resources.FindObjectsOfTypeAll(typeof(AudioListener));
        if(audioListeners.Length == 0)
        {
            gameObject.AddComponent<AudioListener>();
        }

        for (int i = 0; i < sons.Length; i++)
        {
            if(sons[i].source == null)
            {
                sons[i].source = gameObject.AddComponent<AudioSource>();
                //print(sons[i].clip.name + " : " + sons[i].source);
                sons[i].source.clip = sons[i].clip;
                sons[i].source.volume = sons[i].volume;
                sons[i].source.loop = sons[i].loop;
                sons[i].source.playOnAwake = sons[i].playOnAwake;

                if (sons[i].playOnAwake)
                {
                    Play(sons[i].clip.name);
                }
            }
        }
    }


    public void Play(string name)
    {
        Son s = Array.Find(sons, son => son.clip.name == name);

        if (s != null)
        {
            s.clip.LoadAudioData();
            s.source.Play();

        }
        else
            Debug.Log($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
    }
    public void Stop(string name)
    {
        Son s = Array.Find(sons, son => son.clip.name == name);

        if (s != null)
        {
            s.clip.UnloadAudioData();
            s.source.Stop();

        }
        else
            Debug.Log($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
    }







    public void Play(AudioClip clip)
    {
        Son s = Array.Find(sons, son => son.clip == clip);

        if (s != null)
        {
            s.clip.LoadAudioData();
            s.source.Play();

        }
        else
            Debug.Log($"Erreur : Le clip \"{clip.name}\" n'existe pas dans la liste des sons.");
    }
    public void Stop(AudioClip clip)
    {
        Son s = Array.Find(sons, son => son.clip == clip);

        if (s != null)
        {
            s.clip.UnloadAudioData();
            s.source.Stop();

        }
        else
            Debug.Log($"Erreur : Le clip \"{clip.name}\" n'existe pas dans la liste des sons.");
    }








    public void PlayRandomSoundFromList(int[] indexs)
    {
        int alea = UnityEngine.Random.Range(0, indexs.Length);
        Son s = sons[indexs[alea]];

        if (s != null)
        {
            s.clip.LoadAudioData();
            s.source.Play();

        }
        else
            Debug.Log($"Erreur : L'ID n° \"{indexs[alea]}\" n'existe pas dans la liste des sons.");
    }


    public void PlayRandomSoundFromList(string[] noms)
    {
        int alea = UnityEngine.Random.Range(0, noms.Length);
        Son s = Array.Find(sons, son => son.clip.name == noms[alea]);

        if (s != null)
        {
            s.clip.UnloadAudioData();
            s.source.Stop();

        }
        else
            Debug.Log($"Erreur : Le nom \"{noms[alea]}\" n'existe pas dans la liste des sons.");
    }





    public Son GetSonFromClip(AudioClip clip)
    {
        Son s = Array.Find(sons, son => son.clip == clip);

        if (s != null)
            return s;
        else
        {
            Debug.Log($"Erreur : Le clip \"{clip.name}\" n'existe pas dans la liste des sons.");
            return null;
        }
    }

    public Son GetSonFromName(string name)
    {
        Son s = Array.Find(sons, son => son.clip.name == name);

        if (s != null)
            return s;
        else
        {
            Debug.Log($"Erreur : Le clip \"{name}\" n'existe pas dans la liste des sons.");
            return null;
        }
    }
    public Son GetSonFromNameContains(string str)
    {
        Son s = Array.Find(sons, son => son.clip.name.Contains(str));

        if (s != null)
            return s;
        else
        {
            Debug.Log($"Erreur : Le clip \"{name}\" n'existe pas dans la liste des sons.");
            return null;
        }
    }
    public Son[] GetAllSonsWhereNameContains(string str)
    {
        List<Son> sonsTrouvés = new List<Son>();

        for (int i = 0; i < sons.Length; i++)
        {
            if(sons[i] == Array.Find(sons, son => son.clip.name.Contains(str)) && !sonsTrouvés.Contains(sons[i]))
            {
                sonsTrouvés.Add(sons[i]);
            }
        }


        if (sonsTrouvés.Count > 0)
            return sonsTrouvés.ToArray();
        else
        {
            Debug.Log($"Erreur : Aucun clip ne contient le mot \"{str}\". Revérifiez l'orthographe et réessayez.");
            return new List<Son>().ToArray();
        }
    }


    public Son GetSonFromTag(string tag)
    {
        Son s = Array.Find(sons, son => son.tag == tag);

        if (s != null)
            return s;
        else
        {
            Debug.Log($"Erreur : Le clip \"{tag}\" n'existe pas dans la liste des sons.");
            return null;
        }
    }
}