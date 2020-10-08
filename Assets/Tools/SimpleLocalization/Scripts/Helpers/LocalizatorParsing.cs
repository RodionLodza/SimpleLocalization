using System.Collections.Generic;
using UnityEngine;
using System;

namespace SimpleLocalization.Helpers
{
    public static class LocalizatorParsing
    {
        public static List<LocalizedLanguageElement> ParseTranslationFile()
        {
            TextAsset translationsFile = LocalizatorLocalFiles.ReadLocalizationFile();
            List<LocalizedLanguageElement> localizedLanguages = new List<LocalizedLanguageElement>();

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
                }
                else
                {
                    Debug.LogWarning("<color=red>SIMPLE-LOCALIZATOR ERROR</color>: Translations file is empty!");
                }
            }
            else
            {
                Debug.LogWarning("<color=red>SIMPLE-LOCALIZATOR ERROR</color>: Translations file doesn't exist!");
            }

            return localizedLanguages;
        }

        private static SystemLanguage ParseSystemLanguage(string languageName)
        {
            if (!Enum.IsDefined(typeof(SystemLanguage), languageName))
                return SystemLanguage.Unknown;

            return (SystemLanguage)Enum.Parse(typeof(SystemLanguage), languageName);
        }
    }
}