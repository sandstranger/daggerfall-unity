using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaggerfallWorkshop
{
    sealed class AndroidManager : MonoBehaviour
    {
        private void Start()
        {
            CopyStreamingAssetsToInternalMemory();

            SceneManager.LoadScene(1);
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