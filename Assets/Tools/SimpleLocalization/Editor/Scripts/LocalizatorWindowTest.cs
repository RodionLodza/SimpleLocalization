#if UNITY_EDITOR

using SimpleLocalization.Settings;
using SimpleLocalization;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class LocalizatorWindowTest : EditorWindow
{
    [SerializeField] private Texture logo = null;

    private static string releaseTableLink = string.Empty;
    private static string developmentTableLink = string.Empty;
    private static int downloadingTimeout = 0;
    private static DownloadingType downloadingType = default;
    private static PreprocessBuildDownloadingType preprocessBuildDownloadingType = default;

    private string[] cashLanguages = null;
    private int selectedLanguageIndex = 0;

    private int tabIndex = 0;
    private string[] tabs = { "Settings", "Loading", "Parsing", "Testing" };


    private bool hasParsingWarnings = false;
    private bool correctlyParsed = false;
    private bool correctlySaved = false;

    [MenuItem("Tools/SimpleLocalizationTest")]
    public static void ShowWindow()
    {
        LoadSettings();
        GetWindowWithRect<LocalizatorWindowTest>(new Rect(0f, 0f, 370, 270), false, "SimpleLocalization", true);
    }

    private static void LoadSettings()
    {
        LocalizatorSettings.LoadSettings();
        releaseTableLink = LocalizatorSettings.ReleaseTableLink;
        developmentTableLink = LocalizatorSettings.DevelopmentTableLink;
        downloadingTimeout = LocalizatorSettings.DownloadingTimeout;
    }

    private void ShowLogo()
    {
        GUI.DrawTexture(new Rect(position.width / 2 - 138, 0, 276, 51), logo, ScaleMode.ScaleToFit, true, 10.0F);
        DrawSpace(8);
    }

    private void DrawSpace(int count)
    {
        for (int i = 0; i < count; i++)
        {
            EditorGUILayout.Space();
        }
    }

    private void DrawCloseButton()
    {
        DrawSpace(2);
        EditorGUILayout.BeginHorizontal();
        DrawSpace(3);
        if (GUILayout.Button("Close"))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnGUI()
    {
        ShowLogo();
        tabIndex = GUILayout.Toolbar(tabIndex, tabs);

        switch (tabIndex)
        {
            case 0:
                {
                    ShowSettingsTab();
                }
                break;
            case 1:
                {
                }
                break;
            case 2:
                {
                    ShowParsingTab();
                }
                break;
            case 3:
                {
                    ShowTestingTab();
                }
                break;
            default:
                break;
        }
    }

    #region Settings tab

    private void ShowSettingsTab()
    {
        DrawSpace(1);
        releaseTableLink = EditorGUILayout.TextField("Release table link: ", releaseTableLink);
        developmentTableLink = EditorGUILayout.TextField("Development table link: ", developmentTableLink);

        int.TryParse(EditorGUILayout.TextField("Downloading timeout: ", downloadingTimeout.ToString()), out downloadingTimeout);
        
        var popupStyle = GUI.skin.GetStyle("Popup");
        popupStyle.fontSize = 11;

        var downloadingTypes = Enum.GetNames(typeof(DownloadingType)).ToArray();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Downloading type:");
        downloadingType = (DownloadingType)EditorGUILayout.Popup((int)downloadingType, downloadingTypes, popupStyle);
        EditorGUILayout.EndHorizontal();

        var preprocessBuildDownloadingTypes = Enum.GetNames(typeof(PreprocessBuildDownloadingType)).ToArray();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Preprocess build downloading type:");
        preprocessBuildDownloadingType = (PreprocessBuildDownloadingType)EditorGUILayout.Popup((int)preprocessBuildDownloadingType, preprocessBuildDownloadingTypes, popupStyle);
        EditorGUILayout.EndHorizontal();

        DrawSpace(3);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save settings"))
        {
            LocalizatorSettings.SetSettings(releaseTableLink, developmentTableLink, downloadingType, preprocessBuildDownloadingType, downloadingTimeout);
            LocalizatorSettings.SaveSettings();
            correctlySaved = true;
        }
        if (GUILayout.Button("Close"))
        {
            Close();
        }
        EditorGUILayout.EndHorizontal();

        if (correctlySaved)
        {
            EditorGUILayout.HelpBox("Save successful.", MessageType.Info);
        }
    }

    #endregion

    #region Testing tab

    private void ShowTestingTab()
    {
        DrawSpace(1);
        ShowGeneralInformation();
        ShowTestTranslationsButtons();
        DrawCloseButton();
    }

    private void ShowGeneralInformation()
    {
        var availableLanguages = Localizator.GetAvailableLanguages();
        cashLanguages = new string[availableLanguages.Length];

        for (int i = 0; i < availableLanguages.Length; i++)
        {
            cashLanguages[i] = availableLanguages[i].ToString();
        }

        var italicStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
        italicStyle.fontStyle = FontStyle.Italic;
        italicStyle.alignment = TextAnchor.LowerLeft;

        EditorGUILayout.LabelField("General information", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current language: ", Localizator.GetCurrentUseLanguage(), italicStyle);
        EditorGUILayout.LabelField("Available languages: ", availableLanguages.Length.ToString(), italicStyle);
        DrawSpace(2);
    }

    private void ShowTestTranslationsButtons()
    {
        DrawSpace(1);
        EditorGUILayout.LabelField("Changing language", EditorStyles.boldLabel);
        var popupStyle = GUI.skin.GetStyle("Popup");
        popupStyle.fontSize = 11;

        EditorGUILayout.BeginHorizontal();
        selectedLanguageIndex = EditorGUILayout.Popup(selectedLanguageIndex, cashLanguages, popupStyle);
        if (GUILayout.Button("Set selected language"))
        {
            Localizator.ChangeLanguage(selectedLanguageIndex);
        }
        EditorGUILayout.EndHorizontal();
        DrawSpace(1);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Set next language"))
        {
            Localizator.ChangeLanguage();
        }

        if (GUILayout.Button("Set default language"))
        {
            Localizator.SetDefaultLanguage();
        }
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Parsing tab

    private void ShowParsingTab()
    {
        DrawSpace(1);
        ShowParsingButton();
        ShowParsingInfo();
        DrawCloseButton();
    }

    private void ShowParsingButton()
    {
        EditorGUILayout.LabelField("File parsing", EditorStyles.boldLabel);
        if (GUILayout.Button("Parse translations file"))
        {
            hasParsingWarnings = Localizator.ParseTranslationFileWithReport();

            if (!hasParsingWarnings)
            {
                correctlyParsed = true;
            }
        }

        if (hasParsingWarnings)
        {
            EditorGUILayout.HelpBox("Localizator has parsing errors. See console for details.", MessageType.Error);
        }

        if (correctlyParsed)
        {
            EditorGUILayout.HelpBox("Parsing successful.", MessageType.Info);
        }
    }

    private void ShowParsingInfo()
    {
        // TODO Add general parsing info, keys and values
    }

    #endregion
}

#endif