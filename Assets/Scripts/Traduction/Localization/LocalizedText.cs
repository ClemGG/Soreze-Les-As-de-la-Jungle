using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Ce Component se place sur le même GameObject portant le Component de texte
public class LocalizedText : MonoBehaviour {

    public string key;

    /* Les composants text qui contiendront la traduction.
     * Comme on ne sait pas forcément lequel des deux sera utilisé, on les déclare tous les deux
     * et on n'assigne dans l'inspector que celui dont on a besoin
     */
    [SerializeField] protected Text text;
    [SerializeField] protected TextMeshProUGUI textMesh;


    // Use this for initialization
    public void ChangeText () {

        //On rajoute quand même une ligne de code pour récupérer automatiquement les Composants si jamais
        //on oublie de les assigner
        if (text == null)
        {
            text = GetComponent<Text>();
        }
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        //Si on a un de ces deux Components, on va chercher le LocalizationManager pour y piocher la traduction
        if (text) text.text = LocalizationManager.instance.GetLocalizedData(key);
        if (textMesh) textMesh.text = LocalizationManager.instance.GetLocalizedData(key);
    }

}
