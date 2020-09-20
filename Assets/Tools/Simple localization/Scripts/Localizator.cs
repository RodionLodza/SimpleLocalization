using System.Collections.Generic;
using SimpleLocalization.Helpers;
using UnityEngine;
using System.IO;
using System;

namespace SimpleLocalization
{
    public static class Localizator
    {
        public static Action OnLanguageChanged;

        private const string NameTranslationsFile = "Translations.txt";
        private const string ForceSetLanguage = "ForceSetLanguage";

        private static List<SystemLanguage> languages = new List<SystemLanguage>();
        private static List<LocalizedTextElement> localizedTexts = new List<LocalizedTextElement>();

        static Localizator()
        {
            ParseTranslationFile();
        }

        #region General methods

        private static void ParseTranslationFile()
        {
            TextAsset translationsFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(NameTranslationsFile));

            if (translationsFile != null)
            {
                string[] translationsLine = translationsFile.text.Split('\n');
                if (translationsLine.Length > 0)
                {
                    string[] line = translationsLine[0].Trim().Split('	');

                    if (line.Length > 1)
                        for (int i = 0; i < line.Length; i++)
                            languages.Add(ParseSystemLanguage(line[i]));

                    for (int i = 1; i < translationsLine.Length; i++)
                    {
                        line = translationsLine[i].Trim().Split('	');
                        if (line.Length > 1 && line[0].Length > 0)
                        {
                            LocalizedTextElement newTranslations = new LocalizedTextElement(line[0].Trim(), languages.Count);

                            for (int j = 1; j < line.Length; j++)
                                newTranslations.Translations[j - 1] = line[j].Trim().NewlineReplacer();

                            localizedTexts.Add(newTranslations);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Translations file is empty!");
                }
            }
            else
            {
                Debug.LogWarning("Translations file doesn't exist!");
            }
        }

        private static SystemLanguage ParseSystemLanguage(string languageName)
        {
            if (!Enum.IsDefined(typeof(SystemLanguage), languageName))
                return SystemLanguage.Unknown;

            return (SystemLanguage)Enum.Parse(typeof(SystemLanguage), languageName);
        }

        /// <summary>
        /// Return translated text by key for current language.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="caseType"></param>
        /// <returns></returns>
        public static string Translate(string key, CaseType caseType = CaseType.Default)
        {
            LocalizedTextElement localizedText = localizedTexts.Find(x => x.Key == key);
            return localizedText is null ? key 
                : SetCaseType(localizedText.Translations[IsLanguageExsits(GetCurrentLanguage())], caseType);
        }

        /// <summary>
        /// Return translated text by key for specific language.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="caseType"></param>
        /// <returns></returns>
        public static string Translate(string key, SystemLanguage language, CaseType caseType = CaseType.Default)
        {
            LocalizedTextElement localizedText = localizedTexts.Find(x => x.Key == key);
            return localizedText is null ? key 
                : SetCaseType(localizedText.Translations[IsLanguageExsits(language)], caseType);
        }

        private static int IsLanguageExsits(SystemLanguage language)
        {
            int lanuageIndex = languages.IndexOf(language);
                return lanuageIndex == -1 ? 0 : lanuageIndex;
        }

        #endregion 

        #region Text formatting methods

        private static string SetCaseType(string translatedText, CaseType caseType)
        {
            switch (caseType)
            {
                case CaseType.Default:
                    return translatedText;
                case CaseType.Uppercase:
                    return translatedText.ToUpper();
                case CaseType.Capitalize:
                    return translatedText.ToCapitalize();
                case CaseType.Lowercase:
                    return translatedText.ToLower();
                default:
                    return translatedText;
            }
        }

        private static string ToCapitalize(this string translatedText)
        {
            translatedText.ToLower();
            char.ToUpper(translatedText[0]);
            return translatedText;
        }

        private static string NewlineReplacer(this string translatedText)
        {
            return translatedText.Replace("<br>", "\n");
        }

        #endregion

        #region Change language methods

        /// <summary>
        /// Change current player language (for using in game).
        /// </summary>
        /// <param name="language"></param>
        public static void ChangeLanguage(SystemLanguage language)
        {
            PlayerPrefs.SetInt(ForceSetLanguage, (int)language);
            OnLanguageChanged?.Invoke();
        }

        private static SystemLanguage GetCurrentLanguage()
        {
            if (PlayerPrefs.HasKey(ForceSetLanguage))
                return (SystemLanguage)PlayerPrefs.GetInt(ForceSetLanguage);
            else
                return Application.systemLanguage;
        }

        #endregion

        #region Editor and test methods

        /// <summary>
        /// Change current player language (for test only - any mobile test button).
        /// </summary>
        public static void ChangeLanguage()
        {
            if (languages.Count == 0)
            {
                Debug.LogWarning("Localizator didn't find any language in translations file.");
                return;
            }

            int indexCurrentLanguage = languages.IndexOf(GetCurrentLanguage());
            int indexNextLanguage = (indexCurrentLanguage + 1) % languages.Count;
            ChangeLanguage(languages[indexNextLanguage]);

            OnLanguageChanged?.Invoke();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Change current player language (for editor test only).
        /// </summary>
        /// <param name="indexLanguage"></param>
        public static void ChangeLanguage(int indexLanguage)
        {
            if (languages.Count == 0)
            {
                Debug.LogWarning("Localizator didn't find any language in translations file.");
                return;
            }

            ChangeLanguage(languages[indexLanguage]);

            OnLanguageChanged?.Invoke();
        }

        /// <summary>
        /// Return list parsed languages (for editor test only).
        /// </summary>
        /// <returns></returns>
        public static List<SystemLanguage> GetAvailableLanguages()
        {
            if (languages.Count == 0)
                ParseTranslation();
            return languages;
        }

        /// <summary>
        /// Parse tranlation file (for editor test only).
        /// </summary>
        public static void ParseTranslation()
        {
            ParseTranslationFile();
        }

        /// <summary>
        /// Set default device language (for editor test only).
        /// </summary>
        public static void SetDefaultLanguage()
        {
            PlayerPrefs.DeleteKey(ForceSetLanguage);
            OnLanguageChanged?.Invoke();
        }

        /// <summary>
        /// Return current use language (for editor test only).
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUseLanguage()
        {
            return GetCurrentLanguage().ToString();
        }
#endif
        #endregion
    }

    public enum CaseType
    {
        Default,
        Uppercase,
        Capitalize,
        Lowercase
    }
}

namespace SimpleLocalization.Helpers
{
    public class LocalizedTextElement
    {
        public string Key { get; set; }
        public string[] Translations { get; set; }

        public LocalizedTextElement(string key, int languagesCount)
        {
            this.Key = key;
            Translations = new string[languagesCount];
        }
    }
}