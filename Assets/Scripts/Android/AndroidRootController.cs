using System.Threading.Tasks;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using System.IO;
using UnityEngine.Networking;
#endif
using UnityEngine.SceneManagement;

namespace DaggerfallWorkshop.Game
{
    sealed class AndroidRootController
    {
        const string AssetsCopiedToInternalMemoryKey = "assets_were_copied_to_internal_memory";
        public async Task CopyStreamingAssetsToInternalMemory()
        {
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

                    while (!asyncOperation.isDone)
                    {
                        await Task.Yield();
                    }

                    var directoryPath = Path.Combine(DaggerfallUnityApplication.PersistentDataPath,Path.GetDirectoryName(path));

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var finalPathToAsset = Path.Combine(directoryPath, Path.GetFileName(path));
                    File.WriteAllBytes(finalPathToAsset,loadingRequest.downloadHandler.data);
                }
            }
#endif
            PlayerPrefs.SetInt(AssetsCopiedToInternalMemoryKey, 1);
        }

        public async Task StartGame()
        {
            if (PlayerPrefs.GetInt(AssetsCopiedToInternalMemoryKey, 0) == 0)
            {
                await CopyStreamingAssetsToInternalMemory();
            }

            var asyncOperation = SceneManager.LoadSceneAsync(1);

            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}