#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SimpleLocalization.Testing
{
    public class LocalizatorWindow : EditorWindow
    {
        [SerializeField] private int selectedLanguageIndex = 0;
        [SerializeField] private Texture logo = null;

        private string[] displayedOptions = null;

        [MenuItem("Tools/SimpleLocalization")]
        public static void ShowWindow()
        {
            GetWindowWithRect<LocalizatorWindow>(new Rect(0f, 0f, 300, 280), false, "SimpleLocalization", true);
        }

        private void OnGUI()
        {
            ShowLogo();
            ShowGeneralInformation();
            ShowTestTranslationsButtons();
        }

        private void ShowLogo()
        {
            GUI.DrawTexture(new Rect(position.width / 2 - 138, 0, 276, 51), logo, ScaleMode.ScaleToFit, true, 10.0F);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private void ShowGeneralInformation()
        {
            var availableLanguages = Localizator.GetAvailableLanguages();
            displayedOptions = new string[availableLanguages.Count];

            for (int i = 0; i < availableLanguages.Count; i++)
                displayedOptions[i] = availableLanguages[i].ToString();

            var italicStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            italicStyle.fontStyle = FontStyle.Italic;
            italicStyle.alignment = TextAnchor.LowerLeft;

            EditorGUILayout.LabelField($"General information", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Current language: ", Localizator.GetCurrentUseLanguage(), italicStyle);
            EditorGUILayout.LabelField($"Parsed languages: ", availableLanguages.Count.ToString(), italicStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"File parsing", EditorStyles.boldLabel);
            if (GUILayout.Button("Parse translations file"))
                Localizator.ParseTranslation();
            if (availableLanguages.Count == 0)
                EditorGUILayout.HelpBox("Localizator didn't find any language in translations file.", MessageType.Warning);
        }

        private void ShowTestTranslationsButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Changing language", EditorStyles.boldLabel);
            var popupStyle = GUI.skin.GetStyle("Popup");
            popupStyle.fontSize = 11;

            EditorGUILayout.BeginHorizontal();
            selectedLanguageIndex = EditorGUILayout.Popup(selectedLanguageIndex, displayedOptions, popupStyle);
            if (GUILayout.Button("Set selected language"))
                Localizator.ChangeLanguage(selectedLanguageIndex);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Set next language"))
                Localizator.ChangeLanguage();

            if (GUILayout.Button("Set default language"))
                Localizator.SetDefaultLanguage();
        }
    }
}

#endif