#if UNITY_EDITOR

using SimpleLocalization.Settings;
using UnityEditor.Build.Reporting;
using SimpleLocalization.Helpers;
using UnityEditor.Build;
using UnityEngine;

public class LocalizatorBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("PreprocessBuild: Downloading localization started.");
        LocalizatorSettingsWrapper.LoadSettings(CheckDownloadingLocalization);
    }

    private void CheckDownloadingLocalization()
    {
        switch (LocalizatorSettingsWrapper.PreprocessingDownloading)
        {
            case PreprocessBuildDownloading.NotUse:
                break;
            case PreprocessBuildDownloading.OnlyDevelopmentBuild:
                {
#if DEVELOPMENT_BUILD
                    LocalizatorWebLoader.ForceDownloadTranslationFile(DownloadingFinishedHandler);
#endif
                }
                break;
            case PreprocessBuildDownloading.OnlyReleaseBuild:
                {
#if !DEVELOPMENT_BUILD
                    LocalizatorWebLoader.ForceDownloadTranslationFile(DownloadingFinishedHandler);
#endif
                }
                break;
            case PreprocessBuildDownloading.AnyBuild:
                {
                    LocalizatorWebLoader.ForceDownloadTranslationFile(DownloadingFinishedHandler);
                }
                break;
            default:
                break;
        }
    }

    private void DownloadingFinishedHandler()
    {
        Debug.Log("PreprocessBuild: Downloading localization finished.");
    }
}

#endif