using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaggerfallWorkshop
{
    sealed class AndroidManager : MonoBehaviour
    {
        private void Start()
        {
            RequestManageAllFilesAccess();

            CopyStreamingAssetsToInternalMemory();

            SceneManager.LoadScene(1);
        }

        //https://stackoverflow.com/a/76256946
        private void RequestManageAllFilesAccess()
        {
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
        }

        private void CopyStreamingAssetsToInternalMemory()
        {
            const string assetsCopiedToInternalMemoryKey = "assets_were_copied_to_internal_memory";

            if (PlayerPrefs.GetInt(assetsCopiedToInternalMemoryKey, 0) != 0)
            {
                return;
            }

            BetterStreamingAssets.Initialize();

            string[] paths = BetterStreamingAssets.GetFiles("\\", "*", SearchOption.AllDirectories);

            foreach (var path in paths)
            {
                var directoryPath = Path.Combine(Application.persistentDataPath,Path.GetDirectoryName(path));

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var finalPathToAsset = Path.Combine(directoryPath, Path.GetFileName(path));

                var bytes = BetterStreamingAssets.ReadAllBytes(path);

                File.WriteAllBytes(finalPathToAsset,bytes);
            }

            PlayerPrefs.SetInt(assetsCopiedToInternalMemoryKey, 1);
        }
    }
}