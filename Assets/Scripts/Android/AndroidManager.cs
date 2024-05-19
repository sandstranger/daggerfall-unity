using System.Collections;
using System.IO;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Networking;
#endif
using UnityEngine.SceneManagement;

namespace DaggerfallWorkshop
{
    sealed class AndroidManager : MonoBehaviour
    {
        private void Start()
        {
            RequestManageAllFilesAccess();
            StartCoroutine(CopyStreamingAssetsToInternalMemory());
        }

        //https://stackoverflow.com/a/76256946
        private void RequestManageAllFilesAccess()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                using (var buildCodes = new AndroidJavaClass("android.os.Build$VERSION_CODES"))
                {
//Check SDK version > 29
                    if (buildVersion.GetStatic<int>("SDK_INT") > buildCodes.GetStatic<int>("Q"))
                    {
                        using (var environment = new AndroidJavaClass("android.os.Environment"))
                        {
                            //сhecking if permission already exists
                            if (!environment.CallStatic<bool>("isExternalStorageManager"))
                            {
                                using (var settings = new AndroidJavaClass("android.provider.Settings"))
                                {
                                    using (var uri = new AndroidJavaClass("android.net.Uri"))
                                    {
                                        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                                        {
                                            using (var currentActivity =
                                                   unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                                            {
                                                using (var parsedUri =
                                                       uri.CallStatic<AndroidJavaObject>("parse",
                                                           $"package:{Application.identifier}"))
                                                {
                                                    using (var intent = new AndroidJavaObject("android.content.Intent",
                                                               settings.GetStatic<string>(
                                                                   "ACTION_MANAGE_APP_ALL_FILES_ACCESS_PERMISSION"),
                                                               parsedUri))
                                                    {
                                                        currentActivity.Call("startActivity", intent);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
#endif
        }

        private IEnumerator CopyStreamingAssetsToInternalMemory()
        {
            const string assetsCopiedToInternalMemoryKey = "assets_were_copied_to_internal_memory";

            if (PlayerPrefs.GetInt(assetsCopiedToInternalMemoryKey, 0) != 0)
            {
                LoadGameScene();
                yield break;
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            var paths = Resources.Load<TextAsset>("StreamingAssetsPaths");
            foreach (var path in paths.text.Split('\n'))
            {
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                using (var loadingRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, path)))
                {
                    var asyncOperation = loadingRequest.SendWebRequest();

                    yield return asyncOperation;

                    var directoryPath = Path.Combine(Application.persistentDataPath,Path.GetDirectoryName(path));

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var finalPathToAsset = Path.Combine(directoryPath, Path.GetFileName(path));
                    File.WriteAllBytes(finalPathToAsset,loadingRequest.downloadHandler.data);
                }
            }
#endif
            PlayerPrefs.SetInt(assetsCopiedToInternalMemoryKey, 1);
            LoadGameScene();
        }

        private void LoadGameScene()
        {
            SceneManager.LoadScene(1);
        }
    }
}