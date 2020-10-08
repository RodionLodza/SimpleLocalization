using UnityEngine;
using System.IO;
using System;

namespace SimpleLocalization.Settings
{
    public class LocalizatorSettings
    {
        public string releaseTableLink = null;
        public string developmentTableLink = null;
        public DownloadingType downloadingType = default;
        public PreprocessBuildDownloading preprocessingDownloading = default;
        public int downloadingTimeout = 5;
    }

    public class LocalizatorSettingsWrapper
    {
        private static LocalizatorSettings localizatorSettings = null;
        private static string settingsFilePath = "Assets/Resources/SimpleLocalization/LocalizatorSettings.json";

        #region Properties

        public static string ActualTableLink
        {
            get
            {
                string link = localizatorSettings.developmentTableLink;

#if !DEVELOPMENT_BUILD
            link = localizatorSettings.releaseTableLink;
#endif
                return link;
            }
        }

        public static string ReleaseTableLink
        {
            get
            {
                return localizatorSettings.releaseTableLink;
            }
        }

        public static string DevelopmentTableLink
        {
            get
            {
                return localizatorSettings.developmentTableLink;
            }
        }

        public static DownloadingType DownloadingType
        {
            get
            {
                return localizatorSettings.downloadingType;
            }
        }

        public static PreprocessBuildDownloading PreprocessingDownloading
        {
            get
            {
                return localizatorSettings.preprocessingDownloading;
            }
        }

        public static int DownloadingTimeout
        {
            get
            {
                return localizatorSettings.downloadingTimeout;
            }
        }

#endregion

#region General methods

        public static void LoadSettings(Action onLoadingSettingFinished = null)
        {
            TextAsset settingsFile = Resources.Load<TextAsset>("SimpleLocalization/LocalizatorSettings");
            var settingsString = settingsFile.text;
            localizatorSettings = JsonUtility.FromJson<LocalizatorSettings>(settingsString);
            onLoadingSettingFinished?.Invoke();
        }

        public static void SaveSettings(Action onSavingSettingFinished = null)
        {
            var settingsString = JsonUtility.ToJson(localizatorSettings);
            StreamWriter writer = new StreamWriter(settingsFilePath, true);
            writer.WriteLine(settingsString);
            writer.Close();
            onSavingSettingFinished?.Invoke();
        }

#endregion
    }

    public enum DownloadingType
    {
        ManualInEditor,
        AutoInEditor,
        AutoOnDevice,
        Always
    }

    public enum PreprocessBuildDownloading
    {
        NotUse,
        OnlyDevelopmentBuild,
        OnlyReleaseBuild,
        AnyBuild
    }
}