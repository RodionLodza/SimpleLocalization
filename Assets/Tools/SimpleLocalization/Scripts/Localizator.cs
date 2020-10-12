using SimpleLocalization.Settings;
using System.Collections.Generic;
using SimpleLocalization.Helpers;
using UnityEngine;
using System.Linq;
using System;

namespace SimpleLocalization
{
    public static class Localizator
    {
        public static event Action OnLanguageChanged = null;

        private const string ForceSetLanguage = "ForceSetLanguage";

        private static List<LocalizedLanguageElement> localizedLanguages = null;
        private static LocalizedLanguageElement cashLocalizedCurrentLanguage = null;

        private static bool isInitialized = false;

        #region General methods

        static Localizator()
        {
            if (!isInitialized)
            {
                ParseTranslationFile();
            }
        }

        public static void Initialize()
        {
            LocalizatorSettings.LoadSettings(LoadTranslationFile);
        }

        private static void LoadTranslationFile()
        {
            LocalizatorWebLoader.DownloadTranslationFile(ParseTranslationFile);
        }

        private static void ParseTranslationFile()
        {
            var parseInfo = LocalizatorParsing.ParseTranslationFile();
            localizedLanguages = parseInfo.localizedLanguages;
            CacheCurrentLanguage();

            isInitialized = true;
        }

        private static void CacheCurrentLanguage()
        {
            cashLocalizedCurrentLanguage = localizedLanguages.Find(x => x.Language == GetCurrentLanguage());
        }

        #endregion

        #region Translations methods

        /// <summary>
        /// Return translated text by key for current language.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="caseType"></param>
        /// <returns></returns>
        public static string Translate(string key, CaseType caseType = CaseType.Default)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("<color=yellow>SIMPLE-LOCALIZATOR ERROR</color>: Localizator isn't initialized!");
                return string.Empty;
            }
            
            string translatedString = cashLocalizedCurrentLanguage.GetLocalizedText(key);
            return translatedString is null ? $"Key '{key}' not found!"
                : translatedString.SetCaseType(caseType);
        }

        /// <summary>
        /// Return translated text by key for specific language.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="caseType"></param>
        /// <returns></returns>
        public static string Translate(string key, SystemLanguage language, CaseType caseType = CaseType.Default)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("<color=yellow>SIMPLE-LOCALIZATOR ERROR</color>: Localizator isn't initialized!");
                return string.Empty;
            }

            LocalizedLanguageElement localizedLanguage = localizedLanguages.Find(x => x.Language == language);
            return localizedLanguage is null ? $"Key '{key}' not found for language {language}!"
                : localizedLanguage.GetLocalizedText(key).SetCaseType(caseType);
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
            {
                currentLanguage = (SystemLanguage)PlayerPrefs.GetInt(ForceSetLanguage);
            }
            else
            {
                currentLanguage = Application.systemLanguage;
            }

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
                Debug.LogWarning("<color=yellow>SIMPLE-LOCALIZATOR ERROR</color>: localizator didn't find any language in translations file.");
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
                Debug.LogWarning("<color=yellow>SIMPLE-LOCALIZATOR ERROR</color>: localizator didn't find any language in translations file.");
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
            {
                ParseTranslation();
            }
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

        /// <summary>
        /// Return tuple with parsing info (for editor only).
        /// </summary>
        /// <returns></returns>
        public static bool ParseTranslationFileWithReport()
        {
            var parseInfo = LocalizatorParsing.ParseTranslationFile();
            localizedLanguages = parseInfo.localizedLanguages;
            CacheCurrentLanguage();

            isInitialized = true;
            return parseInfo.withWarnings;
        }

        /// <summary>
        /// For editor only.
        /// </summary>
        /// <returns></returns>
        public static int GetCountParsedKeys()
        {
            return cashLocalizedCurrentLanguage.GetCountKeys();
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