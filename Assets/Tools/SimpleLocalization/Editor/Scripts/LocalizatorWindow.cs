#if UNITY_EDITOR

using SimpleLocalization.Settings;
using SimpleLocalization.Helpers;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace SimpleLocalization.Editor
{
    public class LocalizatorWindow : EditorWindow
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

        private DownloadingStatus releaseTableDownloadStatus = default;
        private DownloadingStatus developmentTableDownloadStatus = default;

        [MenuItem("Tools/SimpleLocalization")]
        public static void ShowWindow()
        {
            LoadSettings();
            GetWindowWithRect<LocalizatorWindow>(new Rect(0f, 0f, 370, 270), false, "SimpleLocalization", true);
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
            GUI.DrawTexture(new Rect(20, 0, 340, 50), logo, ScaleMode.ScaleToFit, true, 0);
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

        private void CashLanguages()
        {
            if (cashLanguages != null)
            {
                return;
            }

            var availableLanguages = Localizator.GetAvailableLanguages();
            cashLanguages = new string[availableLanguages.Length];

            for (int i = 0; i < availableLanguages.Length; i++)
            {
                cashLanguages[i] = availableLanguages[i].ToString();
            }
        }

        private void OnGUI()
        {
            ShowLogo();
            tabIndex = GUILayout.Toolbar(tabIndex, tabs);
            DrawSpace(1);

            switch (tabIndex)
            {
                case 0:
                    {
                        ShowSettingsTab();
                    }
                    break;
                case 1:
                    {
                        ShowLoadingTab();
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
            CashLanguages();
            releaseTableLink = EditorGUILayout.TextField("Release table link: ", releaseTableLink);
            developmentTableLink = EditorGUILayout.TextField("Development table link: ", developmentTableLink);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Downloading timeout:");
            downloadingTimeout = (int)EditorGUILayout.Slider(downloadingTimeout, 1, 15);
            EditorGUILayout.EndHorizontal();

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
            ShowGeneralInformation();
            ShowTestTranslationsButtons();
            DrawCloseButton();
        }

        private void ShowGeneralInformation()
        {
            CashLanguages();
            var italicStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            italicStyle.fontStyle = FontStyle.Italic;
            italicStyle.alignment = TextAnchor.LowerLeft;

            EditorGUILayout.LabelField("General information", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current language: ", Localizator.GetCurrentUseLanguage(), italicStyle);
            EditorGUILayout.LabelField("Available languages: ", cashLanguages.Length.ToString(), italicStyle);
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
            DrawSpace(1);
            var italicStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            italicStyle.fontStyle = FontStyle.Italic;
            italicStyle.alignment = TextAnchor.LowerLeft;

            EditorGUILayout.LabelField("Parsing information", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Parsed languages: ", cashLanguages.Length.ToString(), italicStyle);
            EditorGUILayout.LabelField("Parsed keys: ", Localizator.GetCountParsedKeys().ToString(), italicStyle);
        }

        #endregion

        #region Loading tab

        private void ShowLoadingTab()
        {
            EditorGUILayout.LabelField("Development table", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Status:", GetDownloadingStatus(developmentTableDownloadStatus), GetDownloadingMessageStyle(developmentTableDownloadStatus));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Download"))
            {
                developmentTableDownloadStatus = DownloadingStatus.Downloading;
                LocalizatorWebLoader.DownloadTranslationFileFromEditorWindow(BuildType.Development, () =>  OnDownloadingEnded(BuildType.Development));
            }
            DrawSpace(2);
            EditorGUILayout.EndHorizontal();

            DrawSpace(2);

            EditorGUILayout.LabelField("Release table", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Status:", GetDownloadingStatus(releaseTableDownloadStatus), GetDownloadingMessageStyle(releaseTableDownloadStatus));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Download"))
            {
                releaseTableDownloadStatus = DownloadingStatus.Downloading;
                LocalizatorWebLoader.DownloadTranslationFileFromEditorWindow(BuildType.Release, () => OnDownloadingEnded(BuildType.Release));
            }
            DrawSpace(2);
            EditorGUILayout.EndHorizontal();
            DrawCloseButton();
        }

        private void OnDownloadingEnded(BuildType buildType)
        {
            switch (buildType)
            {
                case BuildType.Release:
                    {
                        releaseTableDownloadStatus = DownloadingStatus.DownloadingComplete;
                    }
                    break;
                case BuildType.Development:
                    {
                        developmentTableDownloadStatus = DownloadingStatus.DownloadingComplete;
                    }
                    break;
                default:
                    break;
            }
        }

        private string GetDownloadingStatus(DownloadingStatus downloadingStatus)
        {
            switch (downloadingStatus)
            {
                case DownloadingStatus.ReadyToDownload:
                    {
                        return "Ready to download";
                    }
                case DownloadingStatus.Downloading:
                    {
                        return "Downloading...";
                    }
                case DownloadingStatus.DownloadingComplete:
                    {
                        return "Downloading complete";
                    }
                default:
                    {
                        return "";
                    }
            }
        }

        private GUIStyle GetDownloadingMessageStyle(DownloadingStatus downloadingStatus)
        {
            var style = new GUIStyle(GUI.skin.GetStyle("Label"));
            switch (downloadingStatus)
            {
                case DownloadingStatus.ReadyToDownload:
                    {
                        style.normal.textColor = Color.white;
                    }
                    break;
                case DownloadingStatus.Downloading:
                    {
                        style.normal.textColor = Color.yellow;
                    }
                    break;
                case DownloadingStatus.DownloadingComplete:
                    {
                        style.normal.textColor = Color.green;
                    }
                    break;
                default:
                    break;
            }

            return style;
        }

        #endregion
    }


    public enum DownloadingStatus
    {
        ReadyToDownload,
        Downloading,
        DownloadingComplete,
    }
}

#endif