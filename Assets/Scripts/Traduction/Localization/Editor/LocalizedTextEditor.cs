using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class LocalizedTextEditor : EditorWindow {
    
    public LocalizationData localizationData;
    public string fileName;

    private Vector2 scrollPosition;
    private Rect headerSectionRect, dicoSectionRect, settingsSectionRect;
    TextureSettingsScriptableObject editorWindowStyle;
    TextureSettingsScriptableObject[] allAvailableStyles;

    int index = 0;
    int[] indexes;
    string[] options;





    [MenuItem("My Tools/Localized Text Editor")]
    static void Init()
    {

        EditorWindow thisWindow = EditorWindow.GetWindow(typeof(LocalizedTextEditor));
        thisWindow.minSize = new Vector2(500f, 500f);
        thisWindow.Show();



    }



    private void OnEnable()
    {
        index = PlayerPrefs.GetInt("theme", 0);
        allAvailableStyles = Resources.LoadAll<TextureSettingsScriptableObject>("LocalizationEditorScriptableObjects/");

        if(index < 0 && index >= allAvailableStyles.Length)
            index = 0;

        editorWindowStyle = allAvailableStyles[index];
    }

    private void OnFocus()
    {
        allAvailableStyles = Resources.LoadAll<TextureSettingsScriptableObject>("LocalizationEditorScriptableObjects/");
        indexes = new int[allAvailableStyles.Length];
        options = new string[allAvailableStyles.Length];

        if (index < 0 && index >= allAvailableStyles.Length)
            index = 0;

        editorWindowStyle = allAvailableStyles[index];
    }
    private void OnLostFocus()
    {
        OnDestroy();

    }



    private void OnDestroy()
    {
        PlayerPrefs.SetInt("theme", index);
    }




    void DrawLogo()   //Va afficher le logo du header en bas à gauche de l'écran
    {

        Rect logoRect = new Rect(editorWindowStyle.logoScreenResX, Screen.height - editorWindowStyle.logoScreenResY, 
                                  editorWindowStyle.logoSize, 
                                  editorWindowStyle.logoSize);
        GUI.DrawTexture(logoRect, editorWindowStyle.logoTexture);
        
    }

    void DrawLayouts()  //Affiche le header en lui-même
    {

        //Pour la section header
        headerSectionRect = new Rect(0, 0, Screen.width, editorWindowStyle.headerSectionSize);
        GUI.DrawTexture(headerSectionRect, editorWindowStyle.headerSectionTexture);


        //Pour la section dictionnaire
        dicoSectionRect = new Rect(0, editorWindowStyle.headerSectionSize, Screen.width, Screen.height - editorWindowStyle.settingsSectionSize * 3f - editorWindowStyle.settingsSectionSize / 2f);
        GUI.DrawTexture(dicoSectionRect, editorWindowStyle.dicoSectionTexture);

        //Pour la section settings
        settingsSectionRect = new Rect(0, Screen.height - editorWindowStyle.settingsSectionSize, Screen.width, editorWindowStyle.settingsSectionSize);
        GUI.DrawTexture(settingsSectionRect, editorWindowStyle.settingsSectionTexture);


        DrawLogo();
    }

    void DrawHeader()   //Va afficher le contenu du header
    {
        GUILayout.BeginArea(headerSectionRect);

        GUILayout.Space(10);


        //GUILayout.Label("Localization manager");
        GUIStyle titreHeaderStyle = new GUIStyle { fontStyle = editorWindowStyle.titreFontStyle,
                                                   alignment = editorWindowStyle.titreAlignement,
                                                   fontSize = editorWindowStyle.titreFontSize,
                                                 };

        titreHeaderStyle.normal.textColor = editorWindowStyle.titreCouleur;

        GUI.Label(new Rect(0, 0, Screen.width, headerSectionRect.height), "LocalizationManager", titreHeaderStyle);

        GUILayout.Space(20);


            //Si on veut animer la couleur des boutons
            //GUI.backgroundColor = Color.Lerp(Color.magenta, Color.cyan, Mathf.Sin((float)EditorApplication.timeSinceStartup) + .5f);
            //Repaint();

            GUI.backgroundColor = editorWindowStyle.saveButtonColor;


            if (localizationData != null)
            {
                if (GUILayout.Button("Save Data"))
                {
                    SaveLocalizationData();
                }
            }

            GUI.backgroundColor = editorWindowStyle.loadButtonColor;
            if (GUILayout.Button("Load Data"))
            {
                LoadLocalizationData();
            }

            GUI.backgroundColor = editorWindowStyle.newButtonColor;
            if (GUILayout.Button("New Data"))
            {
                CreateNewLocalizationData();
            }

        GUILayout.EndArea();
    }

    void DrawDico()   //Va afficher le contenu du dictionnaire
    {

        GUILayout.BeginArea(dicoSectionRect);


        if (localizationData != null)
        {

            EditorGUILayout.BeginVertical();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(dicoSectionRect.width), GUILayout.Height(dicoSectionRect.height));



            GUI.backgroundColor = editorWindowStyle.backgroundTextColor;

            GUIStyle titreDicoStyle = new GUIStyle { fontStyle = editorWindowStyle.sousTitreFontStyle,
                                                        alignment = editorWindowStyle.sousTitreAlignement,
                                                        fontSize = editorWindowStyle.sousTitreFontSize,
                                                    };

            titreDicoStyle.normal.textColor = editorWindowStyle.sousTitreCouleur;

            GUI.Label(new Rect(0, 5, Screen.width, editorWindowStyle.sousTitreFontSize), "Dictionnaire actuel : " + fileName, titreDicoStyle);

            GUILayout.Space(40);





            fileName = EditorGUILayout.TextField("Nom du fichier : ", fileName);

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("localizationData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();



            GUI.EndScrollView();
            EditorGUILayout.EndVertical();

        }


        GUILayout.EndArea();
    }

    void DrawSettings()
    {
        GUI.backgroundColor = editorWindowStyle.settingsColor;


        GUILayout.BeginArea(settingsSectionRect);


        for (int i = 0; i < options.Length; i++)
        {
            options[i] = allAvailableStyles[i].name;
            indexes[i] = i;
        }

        index = EditorGUI.IntPopup(new Rect(0, 0, 100, 20), index, options, indexes);
        if (GUI.changed)
        {
            editorWindowStyle = allAvailableStyles[index];
        }



        GUILayout.EndArea();
    }



    private void OnGUI()
    {
        //Debug.Log("Screen Width : " + Screen.width + ", Screen Height : " + Screen.height);

        DrawLayouts();
        DrawHeader();
        DrawDico();
        DrawSettings();
    }








    private void CreateNewLocalizationData()
    {
        localizationData = new LocalizationData();
        fileName = EditorGUILayout.TextField("Nom du fichier : ", "traduction_");
    }


    private void SaveLocalizationData()
    {
        //Debug.Log(fileName);

        string filePath = EditorUtility.SaveFilePanel("Save Localization Data File", Application.streamingAssetsPath, fileName, ".json");


        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(localizationData, true);
            File.WriteAllText(filePath, dataAsJson);
        }
    }


    private void LoadLocalizationData()
    {
        string filePath = EditorUtility.OpenFilePanel("Select Localization Data File", Application.streamingAssetsPath, ".json");


        if (!string.IsNullOrEmpty(filePath))
        {
            
            fileName = Path.GetFileName(filePath).Replace("..json", "");
            string dataAsJson = File.ReadAllText(filePath);
            localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
        }
    }
}
