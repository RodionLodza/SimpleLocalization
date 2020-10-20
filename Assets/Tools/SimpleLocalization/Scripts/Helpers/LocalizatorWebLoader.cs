#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

using SimpleLocalization.Settings;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine;
using System;

namespace SimpleLocalization.Helpers
{
    public static class LocalizatorWebLoader
    {
        public static void DownloadTranslationFile(Action onLoadedEnded)
        {
            switch (LocalizatorSettings.DownloadingType)
            {
                case DownloadingType.ManualInEditor:
                    {
                        onLoadedEnded?.Invoke();
                    }
                    break;
                case DownloadingType.AutoInEditor:
                    {
#if !UNITY_EDITOR
                        onLoadedEnded?.Invoke();
#else
                        EditorCoroutineUtility.StartCoroutineOwnerless(StartLoadingFile(LocalizatorSettings.ActualTableLink, onLoadedEnded));
#endif
                    }
                    break;
                case DownloadingType.AutoOnDevice:
                    {
#if UNITY_EDITOR
                        onLoadedEnded?.Invoke();
#else
                        LoadOnDevice(onLoadedEnded);
#endif
                    }
                    break;
                case DownloadingType.Always:
                    {
#if UNITY_EDITOR
                        EditorCoroutineUtility.StartCoroutineOwnerless(StartLoadingFile(LocalizatorSettings.ActualTableLink, onLoadedEnded));
#else
                        LoadOnDevice(onLoadedEnded);
#endif
                    }
                    break;
            }
        }

        private static IEnumerator StartLoadingFile(string link, Action onLoadedEnded)
        {
            UnityWebRequest request = UnityWebRequest.Get(link);

            request.timeout = LocalizatorSettings.DownloadingTimeout;
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogWarning("<color=yellow>SIMPLE-LOCALIZATOR ERROR</color>: Network or http error on downloading localizator file.");
                onLoadedEnded?.Invoke();
            }
            else
            {
                LocalizatorLocalFiles.WriteLocalizationFile(request.downloadHandler.text, onLoadedEnded);
            }

            request.Dispose();
        }

        private static void LoadOnDevice(Action onLoadedEnded)
        {
            GameObject gameObject = new GameObject("LocalizatorWebLoader");
            LocalizatorWebLoaderController localizatorWebLoaderController = gameObject.AddComponent<LocalizatorWebLoaderController>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            onLoadedEnded += () => UnityEngine.Object.Destroy(gameObject);
            localizatorWebLoaderController.StartCoroutine(StartLoadingFile(LocalizatorSettings.ActualTableLink, onLoadedEnded));
        }

#if UNITY_EDITOR

        public static void DownloadTranslationFileFromEditorWindow(BuildType buildType, Action onLoadedEnded)
        {
            switch (buildType)
            {
                case BuildType.Release:
                    {
                        EditorCoroutineUtility.StartCoroutineOwnerless(StartLoadingFile(LocalizatorSettings.ReleaseTableLink, onLoadedEnded));
                    }
                    break;
                case BuildType.Development:
                    {
                        EditorCoroutineUtility.StartCoroutineOwnerless(StartLoadingFile(LocalizatorSettings.DevelopmentTableLink, onLoadedEnded));
                    }
                    break;
                default:
                    break;
            }
        }

#endif
    }

    public class LocalizatorWebLoaderController : MonoBehaviour
    {
    }

    public enum BuildType
    {
        Release,
        Development
    }
}