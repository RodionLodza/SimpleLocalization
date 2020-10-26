using SimpleLocalization.Settings.Data;
using UnityEngine;
using System.IO;
using System;

namespace SimpleLocalization.Settings.Data
{
    public class LocalizatorSettingsData
    {
        public string releaseTableLink = null;
        public string developmentTableLink = null;
        public DownloadingType downloadingType = default;
        public PreprocessBuildDownloadingType preprocessingDownloading = default;
        public int downloadingTimeout = 5;
    }
}

namespace SimpleLocalization.Settings
{
    public class LocalizatorSettings
    {
        private static LocalizatorSettingsData localizatorSettings = null;
        private static string settingsFilePath = "Assets/Resources/SimpleLocalization/LocalizatorSettings.json";

        #region Properties

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

        public static DownloadingType DownloadingType
        {
            get
            {
                return localizatorSettings.downloadingType;
            }
        }

        public static PreprocessBuildDownloadingType PreprocessingDownloading
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

            if (settingsFile == null)
            {
                Debug.LogWarning("<color=yellow>SIMPLE-LOCALIZATOR Warning</color>: Can't find localizator setting file Resources/SimpleLocalization/LocalizatorSettings.json. LocalizatorSettings.json re-created with default values.");
                localizatorSettings = new LocalizatorSettingsData();
            }
            else
            {
                var settingsString = settingsFile.text;
                localizatorSettings = JsonUtility.FromJson<LocalizatorSettingsData>(settingsString);
            }
       
            onLoadingSettingFinished?.Invoke();
        }

        public static void SaveSettings(Action onSavingSettingFinished = null)
        {
            var settingsString = JsonUtility.ToJson(localizatorSettings);
            StreamWriter writer = new StreamWriter(settingsFilePath, false);
            writer.WriteLine(settingsString);
            writer.Close();
            onSavingSettingFinished?.Invoke();
        }

        public static void SetSettings(string releaseTableLink, string developmentTableLink, DownloadingType downloadingType, PreprocessBuildDownloadingType preprocessingDownloading, int downloadingTimeout)
        {
            localizatorSettings.releaseTableLink = releaseTableLink;
            localizatorSettings.developmentTableLink = developmentTableLink;
            localizatorSettings.downloadingType = downloadingType;
            localizatorSettings.preprocessingDownloading = preprocessingDownloading;
            localizatorSettings.downloadingTimeout = downloadingTimeout;
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

    public enum PreprocessBuildDownloadingType
    {
        NotUse,
        OnlyDevelopmentBuild,
        OnlyReleaseBuild,
        AnyBuild
    }
}