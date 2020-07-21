using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalizationManager : MonoBehaviour {

    //Contient la traduction pour la langue actuelle
    public Dictionary<string, string> localizedText;

    [Tooltip("Contient des infos sur le format de fichier utilisé pour la recherche des traductions.")]
    public string fileGenericName = "traduction_", fileExtension = "..json", missingtextString = "Texte localisé non trouvé pour le tag \"{0} \"!";
    public static LocalizationManager instance;


    [Tooltip("Varaible exposée qui ne sert qu'à afficher la langue actuelle.")]
    public string currentLanguage;



    private bool isReady = false;

    //Utilisé par le LocalizationStartupManager pour savoir si ce script a récupéré la traduction depuis le fichier .json
    //et est prêt à traduire
    public bool CheckIfReady()
    {
        return isReady;
    }


    //Singleton
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




    //Récupère la traduction depuis un fichier .json et le stocke dans le dictionnaire localizedText
    //Le fichier a un format précis, donc

    public void LoadLocalizedText(string fileLanguage)
    {
        //On garde en mémoire la nouvelle langue à utiliser
        currentLanguage = fileLanguage;
        //print(LocalizationManager.instance.currentLanguage);


        //On récupère le chemin vers le fichier et on initialiser le dictionnaire
        string fileName = string.Format("{0}{1}{2}", fileGenericName, fileLanguage, fileExtension);
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName).Replace('\\', '/');

        localizedText = new Dictionary<string, string>();


        if (!File.Exists(filePath))
        {
            Debug.Log("Erreur : Le fichier \"" + fileName + "\" est introuvable dans le dossier \"" + filePath + "\".");
        }
        else
        {
            //Si le fichier existe, on le lit en entier et on le convertit en LocalizedData, qui est un tableau contenant chaque traduction et son mot clé de recherche
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            //Ensuite, on ajoute chaque traduction et son mot clé de recherche au dictionnaire
            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
            //Debug.Log("Données chargées, le dictionnaire contient " + localizedText.Count + " entrées.");


            //Et on traduit tous les textes
            LocalizationStartupManager.instance.ChangeAllItemsInScene();
        }

        //On prévient les autres scripts que le Localizationmanager a fini de s'initialiser
        isReady = true;
    }


    //Appelé par les Localizedtext pour récupérer leur traduction dans le dictionnaire en lui passant une clé de référence
    public string GetLocalizedData(string key)
    {
        //Si la clé n'existe pas dans le dictionnaire, on renvoie un message d'erreur à la place de la traduction.
        //Ca nous permettra de savoir exactement quel Component de texte merde et pourquoi

        string result = string.Format(missingtextString, key);

        if (key == null)
            return result;

        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;
    }
}
