#if UNITY_EDITOR

using SimpleLocalization.Settings;
using UnityEditor.Build.Reporting;
using SimpleLocalization.Helpers;
using UnityEngine.Networking;
using UnityEditor.Build;
using UnityEngine;

public class LocalizatorBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("SIMPLE-LOCALIZATOR: preprocess build downloading localization started.");
        LocalizatorSettings.LoadSettings(CheckDownloadingLocalization);
    }

    private void CheckDownloadingLocalization()
    {
        switch (LocalizatorSettings.PreprocessingDownloading)
        {
            case PreprocessBuildDownloadingType.NotUse:
                break;
            case PreprocessBuildDownloadingType.OnlyDevelopmentBuild:
                {
#if DEVELOPMENT_BUILD
                    Debug.Log("PreprocessBuildDownloading.OnlyDevelopmentBuild");
                    DownloadLocalization();
#endif
                }
                break;
            case PreprocessBuildDownloadingType.OnlyReleaseBuild:
                {
#if !DEVELOPMENT_BUILD
                    DownloadLocalization();
#endif
                }
                break;
            case PreprocessBuildDownloadingType.AnyBuild:
                {
                    DownloadLocalization();
                }
                break;
            default:
                break;
        }
    }

    private void DownloadLocalization()
    {
        UnityWebRequest request = UnityWebRequest.Get(LocalizatorSettings.ActualTableLink);

        request.timeout = LocalizatorSettings.DownloadingTimeout;
        request.SendWebRequest();

        while (!request.isDone)
        {
            // waiting finished request
        }

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogWarning("<color=yellow>SIMPLE-LOCALIZATOR ERROR</color>: Network or http error on downloading localizator file.");
        }
        else
        {
            LocalizatorLocalFiles.WriteLocalizationFile(request.downloadHandler.text, DownloadingFinishedHandler);
        }

        request.Dispose();
    }

    private void DownloadingFinishedHandler()
    {
        Debug.Log("SIMPLE-LOCALIZATOR: preprocess build downloading localization finished.");
    }
}

#endif