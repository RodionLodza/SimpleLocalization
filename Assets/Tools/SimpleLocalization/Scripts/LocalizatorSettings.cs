using SimpleLocalization.Settings.Data;
using UnityEngine;
using System.IO;
using System;

namespace SimpleLocalization.Settings.Data
{
    public class LocalizatorSettings
    {
        public string releaseTableLink = null;
        public string developmentTableLink = null;
        public DownloadingType downloadingType = default;
        public PreprocessBuildDownloading preprocessingDownloading = default;
        public int downloadingTimeout = 5;
    }
}

namespace SimpleLocalization.Settings
{
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

            if (settingsFile == null)
            {
                Debug.LogWarning("<color=yellow>SIMPLE-LOCALIZATOR ERROR</color>: Can't find localizator setting file Resources/SimpleLocalization/LocalizatorSettings.json");
                return;
            }

            var settingsString = settingsFile.text;
            localizatorSettings = JsonUtility.FromJson<LocalizatorSettings>(settingsString);
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