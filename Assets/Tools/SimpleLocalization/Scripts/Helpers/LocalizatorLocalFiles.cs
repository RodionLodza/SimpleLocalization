using UnityEngine;
using System.IO;
using System;

namespace SimpleLocalization.Helpers
{
    public static class LocalizatorLocalFiles
    {
        private static readonly string PathTranslationsFile = $"{Application.dataPath}/Resources/SimpleLocalization/Translations.txt";
        private static readonly string PathTranslationsFileDevice = Path.Combine(Application.persistentDataPath, "Translations.txt");

        public static string ReadLocalizationFile()
        {
            string translationsFile = null;
            
#if UNITY_EDITOR
            StreamReader rider = new StreamReader(PathTranslationsFile);
            translationsFile = rider.ReadToEnd();
            rider.Close();
#else
        if (File.Exists(PathTranslationsFileDevice))
        {
            StreamReader rider = new StreamReader(PathTranslationsFileDevice);
            translationsFile = rider.ReadToEnd();
            rider.Close();
        }
        else
        {
            TextAsset translationsTextAsset = default;    
            translationsTextAsset = Resources.Load<TextAsset>("SimpleLocalization/Translations");
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

            StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine(translationsFile);
            writer.Close();

            onWritingEnded?.Invoke();
        }
    }
}