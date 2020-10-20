using System.Collections.Generic;
using UnityEngine;
using System;

namespace SimpleLocalization.Helpers
{
    public static class LocalizatorParsing
    {
        public static (List<LocalizedLanguageElement> localizedLanguages, bool withWarnings) ParseTranslationFile()
        {
            string translationsFile = LocalizatorLocalFiles.ReadLocalizationFile();
            List<LocalizedLanguageElement> localizedLanguages = new List<LocalizedLanguageElement>();
            bool withWarnings = false;

            if (translationsFile != null)
            {
                string[] translationsLine = translationsFile.Split('\n');
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
                        int countTranslationsInLine = line.Length - 1;

                        if (line.Length == 1 && string.Equals(line[0], ""))
                        {
                            continue;
                        }

                        if (countTranslationsInLine != localizedLanguages.Count)
                        {
                            Debug.LogWarning($"<color=yellow>SIMPLE-LOCALIZATOR WARNING</color>: The key '{line[0].Trim()}' is not translated into all languages!");
                            withWarnings = true;
                        }

                        if (line.Length > 1)
                        {
                            for (int k = 0; k < countTranslationsInLine; k++)
                            {
                                localizedLanguages[k].AddTranlsation(new LocalizedTextElement(line[0].Trim(), line[k + 1].Trim().NewlineReplacer()));
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("<color=yellow>SIMPLE-LOCALIZATOR ERROR</color>: Translations file is empty!");
                    withWarnings = true;
                }
            }
            else
            {
                Debug.LogError("<color=yellow>SIMPLE-LOCALIZATOR ERROR</color>: Translations file doesn't exist!");
                withWarnings = true;
            }

            return (localizedLanguages, withWarnings);
        }

        private static SystemLanguage ParseSystemLanguage(string languageName)
        {
            if (!Enum.IsDefined(typeof(SystemLanguage), languageName))
            {
                return SystemLanguage.Unknown;
            }

            return (SystemLanguage)Enum.Parse(typeof(SystemLanguage), languageName);
        }
    }
}