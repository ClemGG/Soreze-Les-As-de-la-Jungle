using UnityEngine;


[System.Serializable]
public class LocalizationData {

    //Le LocalizedData contient toutes les traductions dans un fichier .json et leurs mots clés
    public LocalizationItem[] items;

}



[System.Serializable]
public class LocalizationItem
{  
    //La clé permettant de retrouver sa valeur correspondante
    public string key;

    //La traduction elle-même
    [TextArea(3,10)] public string value;

}
