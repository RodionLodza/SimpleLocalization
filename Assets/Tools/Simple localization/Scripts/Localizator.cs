using System.Collections.Generic;
using SimpleLocalization.Helpers;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

namespace SimpleLocalization
{
    public static class Localizator
    {
        public static Action OnLanguageChanged;

        private const string NameTranslationsFile = "Translations.txt";
        private const string ForceSetLanguage = "ForceSetLanguage";

        private static List<LocalizedLanguageElement> localizedLanguages = new List<LocalizedLanguageElement>();
        private static LocalizedLanguageElement cashLocalizedCurrentLanguage = null;

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
                    localizedLanguages.Clear();
                    string[] line = translationsLine[0].Trim().Split('\t');

                    for (int i = 0; i < line.Length; i++)
                    {
                        LocalizedLanguageElement newLanguage = new LocalizedLanguageElement(ParseSystemLanguage(line[i]));
                        localizedLanguages.Add(newLanguage);
                    }

                    for (int j = 1; j < translationsLine.Length; j++)
                    {
                        line = translationsLine[j].Trim().Split('\t');
                        if (line.Length > 1)
                        {
                            for (int k = 0; k < localizedLanguages.Count; k++)
                            {
                                localizedLanguages[k].AddTranlsation(new LocalizedTextElement(line[0].Trim(), line[k + 1].Trim().NewlineReplacer()));
                            }
                        }
                    }

                    CacheCurrentLanguage();
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
            string translatedString = cashLocalizedCurrentLanguage.GetLocalizedText(key);
            return translatedString is null ? $"Key '{key}' not found!"
                : SetCaseType(translatedString, caseType);
        }

        /// <summary>
        /// Return translated text by key for specific language.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="caseType"></param>
        /// <returns></returns>
        public static string Translate(string key, SystemLanguage language, CaseType caseType = CaseType.Default)
        {
            LocalizedLanguageElement localizedLanguage = localizedLanguages.Find(x => x.Language == language);
            return localizedLanguage is null ? $"Key '{key}' not found for language {language}!"
                : SetCaseType(localizedLanguage.GetLocalizedText(key), caseType);
        }

        private static void CacheCurrentLanguage()
        {
            cashLocalizedCurrentLanguage = localizedLanguages.Find(x => x.Language == GetCurrentLanguage());
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
            CacheCurrentLanguage();
            OnLanguageChanged?.Invoke();
        }

        private static SystemLanguage GetCurrentLanguage()
        {
            SystemLanguage currentLanguage;
            if (PlayerPrefs.HasKey(ForceSetLanguage))
                currentLanguage = (SystemLanguage)PlayerPrefs.GetInt(ForceSetLanguage);
            else
                currentLanguage = Application.systemLanguage;

            return localizedLanguages.Where(x => x.Language == currentLanguage).FirstOrDefault() is null ? 
                localizedLanguages[0].Language : currentLanguage;
        }

        #endregion

        #region Editor and test methods

        /// <summary>
        /// Change current player language (for test only - any mobile test button).
        /// </summary>
        public static void ChangeLanguage()
        {
            if (localizedLanguages.Count == 0)
            {
                Debug.LogWarning("Localizator didn't find any language in translations file.");
                return;
            }

            int indexCurrentLanguage = localizedLanguages.IndexOf(cashLocalizedCurrentLanguage);
            int indexNextLanguage = (indexCurrentLanguage + 1) % localizedLanguages.Count;
            ChangeLanguage(localizedLanguages[indexNextLanguage].Language);

            OnLanguageChanged?.Invoke();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Change current player language (for editor test only).
        /// </summary>
        /// <param name="indexLanguage"></param>
        public static void ChangeLanguage(int indexLanguage)
        {
            if (localizedLanguages.Count == 0)
            {
                Debug.LogWarning("Localizator didn't find any language in translations file.");
                return;
            }

            ChangeLanguage(localizedLanguages[indexLanguage].Language);

            OnLanguageChanged?.Invoke();
        }

        /// <summary>
        /// Return list parsed languages (for editor test only).
        /// </summary>
        /// <returns></returns>
        public static SystemLanguage[] GetAvailableLanguages()
        {
            if (localizedLanguages.Count == 0)
                ParseTranslation();
            return localizedLanguages.Select(x => x.Language).ToArray();
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
            CacheCurrentLanguage();
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
    public class LocalizedLanguageElement
    {
        public SystemLanguage Language { get; set; }

        private List<LocalizedTextElement> translations;

        public LocalizedLanguageElement(SystemLanguage language)
        {
            Language = language;
            translations = new List<LocalizedTextElement>();
        }

        public void AddTranlsation(LocalizedTextElement localizedTextElement)
        {
            translations.Add(localizedTextElement);
        }

        public string GetLocalizedText(string translationKey)
        {
            string localizedString = null;
            foreach (var translation in translations)
            {
                if (string.Equals(translationKey, translation.TranslationKey))
                {
                    localizedString = translation.TranslatedText;
                    break;
                }
            }

            return localizedString;
        }
    }

    public class LocalizedTextElement
    {
        public string TranslationKey = null;
        public string TranslatedText = null;

        public LocalizedTextElement(string translationKey, string translatedText)
        {
            TranslationKey = translationKey;
            TranslatedText = translatedText;
        }
    }
}