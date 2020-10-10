using UnityEngine;
using System.IO;
using System;

namespace SimpleLocalization.Helpers
{
    public static class LocalizatorLocalFiles
    {
        private static readonly string PathTranslationsFile = $"{Application.dataPath}/Resources/SimpleLocalization/Translations.txt";
        private static readonly string PathTranslationsFileInResources = "SimpleLocalization/Translations.txt";
        private static readonly string PathTranslationsFileDevice = $"{Application.persistentDataPath}/Translations.txt";

        public static string ReadLocalizationFile()
        {
            string translationsFile = null;
            TextAsset translationsTextAsset = default;

#if UNITY_EDITOR
            translationsTextAsset = Resources.Load<TextAsset>(PathTranslationsFileInResources);
            translationsFile = translationsTextAsset.text;
#else
        if (File.Exists(PathTranslationsFileDevice))
        {
            StreamReader rider = new StreamReader(PathTranslationsFileDevice);
            translationsFile = rider.ReadToEnd();
            rider.Close();
        }
        else
        {
            translationsTextAsset = Resources.Load<TextAsset>(PathTranslationsFileInResources);
            translationsFile = translationsTextAsset.text;
        }
#endif

            return translationsFile;
        }

        public static void WriteLocalizationFile(string translationsFile, Action onWritingEnded)
        {
            string path = PathTranslationsFile;
#if !UNITY_EDITOR
            path = PathTranslationsFileDevice;
#endif

            StreamWriter writer = new StreamWriter(PathTranslationsFile, false);
            writer.WriteLine(translationsFile);
            writer.Close();

            onWritingEnded?.Invoke();
        }
    }
}