using Unity.EditorCoroutines.Editor;
using SimpleLocalization.Settings;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine;

namespace SimpleLocalization.Helpers
{
    public static class LocalizatorWebLoader
    {
        public static void DownloadTranslationFile(Action onLoadedEnded)
        {
            switch (LocalizatorSettingsWrapper.DownloadingType)
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
                        EditorCoroutineUtility.StartCoroutineOwnerless(StartLoadingFile(LocalizatorSettingsWrapper.ActualTableLink, onLoadedEnded));
#endif
                    }
                    break;
                case DownloadingType.AutoOnDevice:
                    {
#if UNITY_EDITOR
                        onLoadedEnded?.Invoke();
#else
                        //TODO Add downloading with monoBehaviour couroutine
#endif
                    }
                    break;
                case DownloadingType.Always:
                    {
#if UNITY_EDITOR
                        EditorCoroutineUtility.StartCoroutineOwnerless(StartLoadingFile(LocalizatorSettingsWrapper.ActualTableLink, onLoadedEnded));
#else
                        //TODO Add downloading with monoBehaviour couroutine
#endif
                    }
                    break;
            }
        }

        public static void ForceDownloadTranslationFile(Action onLoadedEnded)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(StartLoadingFile(LocalizatorSettingsWrapper.ActualTableLink, onLoadedEnded));
        }

        private static IEnumerator StartLoadingFile(string link, Action onLoadedEnded)
        {
            UnityWebRequest request = UnityWebRequest.Get(link);

            request.timeout = LocalizatorSettingsWrapper.DownloadingTimeout;
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogWarning("<color=red>SIMPLE-LOCALIZATOR ERROR</color>: Network or http error on downloading localizator file.");
                onLoadedEnded?.Invoke();
            }
            else
            {
                LocalizatorLocalFiles.WriteLocalizationFile(request.downloadHandler.text);
                onLoadedEnded?.Invoke();
            }

            request.Dispose();
        }
    }
}