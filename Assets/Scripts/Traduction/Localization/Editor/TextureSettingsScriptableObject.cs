using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Editor Style Template", menuName = "Localization Editor / Editor Style Template")]
public class TextureSettingsScriptableObject : ScriptableObject {




    [Space(10)]
    [Header("Header : ")]
    [Space(10)]


    public Texture2D logoTexture;
    public Texture2D headerSectionTexture;
    public float headerSectionSize, logoSize, logoScreenResX, logoScreenResY;


    [Space(10)]
    [Header("Sections : ")]
    [Space(10)]

    public Texture2D dicoSectionTexture;
    public Texture2D settingsSectionTexture;
    public float dicoSectionSize;
    public float settingsSectionSize;


    [Space(10)]
    [Header("GUIStyles : ")]
    [Space(10)]

    public Color titreCouleur;
    public FontStyle titreFontStyle;
    public TextAnchor titreAlignement;
    public int titreFontSize;

    [Space(10)]

    public Color sousTitreCouleur;
    public FontStyle sousTitreFontStyle;
    public TextAnchor sousTitreAlignement;
    public int sousTitreFontSize;


    [Space(10)]
    [Header("Properties Color : ")]
    [Space(10)]

    public Color saveButtonColor;
    public Color loadButtonColor;
    public Color newButtonColor;
    public Color backgroundTextColor;
    public Color settingsColor;
    public Color propertiesTextColor;
    public Color propertiesNameColor;
    public FontStyle propertiesNameFontStyle;



}
