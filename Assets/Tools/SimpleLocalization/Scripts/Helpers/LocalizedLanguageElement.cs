using System.Collections.Generic;
using UnityEngine;

namespace SimpleLocalization.Helpers
{
    public class LocalizedLanguageElement
    {
        public SystemLanguage Language { get; set; }

        private List<LocalizedTextElement> translations = null;

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

        #region Editor methods

        public int GetCountKeys()
        {
            return translations.Count;
        }

        #endregion
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