#if UNITY_ANDROID && !UNITY_EDITOR
using System.IO;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayerPrefs = DaggerfallWorkshop.Game.PlayerPrefsExtensions;

namespace DaggerfallWorkshop.Game
{
    sealed class AndroidRootController
    {
        const string AssetsCopiedToInternalMemoryKey = "assets_were_copied_to_internal_memory";

        private static readonly string[] _directoriesToCopy = new string[]
        {
            "BIOGs",
            "Books",
            "Docs",
            "Factions",
            "Fonts",
            "GameFiles",
            "Mods",
            "Movies",
            "Presets",
            "QuestPacks",
            "Quests",
            "Sound",
            "SoundFonts",
            "SpellIcons",
            "Tables",
            "Text",
            "WorldData",
            "Textures"
        };

        public void CopyStreamingAssetsToInternalMemory()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var currentActivity =
                       unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (var assetsHelper = new AndroidJavaObject("file.utils.CopyFilesFromAssets",currentActivity))
                    {
                        foreach (var directory in _directoriesToCopy)
                        {
                            assetsHelper.Call("copy",
                                directory,Path.Combine(DaggerfallUnityApplication.PersistentDataPath, directory));
                        }

                    }
                }
            }
#endif
            PlayerPrefs.SetBool(AssetsCopiedToInternalMemoryKey, true);
        }

        public void StartGame()
        {
            bool assetsWereCopied = PlayerPrefs.GetBool(AssetsCopiedToInternalMemoryKey);

            if (!assetsWereCopied)
            {
                CopyStreamingAssetsToInternalMemory();
            }

            SceneManager.LoadScene(1);
        }

        public void ChangeFps(string maxFpsString)
        {
            if (int.TryParse(maxFpsString, out var maxFps))
            {
                DaggerfallUnity.Settings.TargetFrameRate = maxFps;
                DaggerfallUnity.Settings.SaveSettings();
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}