using UnityEngine;
using System.IO;

namespace SimpleLocalization.Helpers
{
    public static class LocalizatorLocalFiles
    {
        private static readonly string PathTranslationsFile = "SimpleLocalization/Translations";
        private static readonly string PathTranslationsFileDevice = $"{Application.persistentDataPath}/Translations";

        public static TextAsset ReadLocalizationFile()
        {
            TextAsset translationsFile = default;

#if UNITY_EDITOR
            translationsFile = Resources.Load<TextAsset>(PathTranslationsFile);
#else
        if (File.Exists(PathTranslationsFileDevice))
        {
            translationsFile = Resources.Load<TextAsset>(PathTranslationsFileDevice);
        }
        else
        {
            translationsFile = Resources.Load<TextAsset>(PathTranslationsFile);
        }
#endif

            return translationsFile;
        }

        public static void WriteLocalizationFile(string translationsFile)
        {

            string path = PathTranslationsFile;
#if !UNITY_EDITOR
            path = PathTranslationsFileDevice;
#endif

            StreamWriter writer = new StreamWriter(PathTranslationsFile, true);
            writer.WriteLine(translationsFile);
            writer.Close();
        }
    }
}