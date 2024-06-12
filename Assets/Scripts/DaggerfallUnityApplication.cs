// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2024 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: kaboissonneault
// Contributors:
//
// Notes:
//

//#define SEPARATE_DEV_PERSISTENT_PATH

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public static class DaggerfallUnityApplication
{
    public const string AndroidRootFolder = "/storage/emulated/0";
    public const string AndroidGameFolder = "/storage/emulated/0/DaggerfallUnity";

    static string persistentDataPath;
    private static bool? isPortableInstall;

    public static bool IsPortableInstall
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return false;
#else
            if (isPortableInstall == null)
            {
                isPortableInstall = !Application.isEditor && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Portable.txt"));
            }

            return isPortableInstall.Value;
#endif
        }
    }

    public static string PersistentDataPath
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return AndroidGameFolder;
#else
            if (persistentDataPath == null)
            {
                InitializePersistentPath();
            }

            return persistentDataPath;
#endif
        }
    }

    public static void CreateAndroidGameFolder()
    {
        if (!Directory.Exists(AndroidGameFolder))
        {
            Directory.CreateDirectory(AndroidGameFolder);
        }
    }

    private static void InitializePersistentPath()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return;
#elif UNITY_EDITOR && SEPARATE_DEV_PERSISTENT_PATH
        persistentDataPath = String.Concat(Application.persistentDataPath, ".devenv");
        Directory.CreateDirectory(persistentDataPath);
#else
        if (IsPortableInstall)
        {
            persistentDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PortableAppdata");
            Directory.CreateDirectory(persistentDataPath);
        }
        else
        {
            persistentDataPath = Application.persistentDataPath;
        }
#endif
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void SubsystemInit()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);
#endif
     //   RequestManageAllFilesAccess();
#if UNITY_ANDROID && !UNITY_EDITOR
        persistentDataPath = AndroidGameFolder;
#else
        if (persistentDataPath == null)
        {
            InitializePersistentPath();
        }
#endif
        InitLog();
    }

    //https://stackoverflow.com/a/76256946
    private static void RequestManageAllFilesAccess()
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

    public class LogHandler : ILogHandler, IDisposable
    {
        private StreamWriter streamWriter;

        public LogHandler()
        {
            string filePath = Path.Combine(persistentDataPath, "Player.log");

            string errorMessage = null;
            try
            {
                if(File.Exists(filePath))
                {
                    string prevPath = Path.Combine(persistentDataPath, "Player-prev.log");
                    File.Delete(prevPath);
                    File.Move(filePath, prevPath);
                }
            }
            catch(Exception e)
            {
                errorMessage = $"Could not preserve previous log: {e.Message}";
            }

            streamWriter = File.CreateText(filePath);
            streamWriter.AutoFlush = true;

            if(!string.IsNullOrEmpty(errorMessage))
            {
                streamWriter.WriteLine(errorMessage);
            }
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            streamWriter.WriteLine(exception.ToString());
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            string prefix = "";
            switch(logType)
            {
                case LogType.Error:
                    prefix = "[Error] ";
                    break;

                case LogType.Warning:
                    prefix = "[Warning] ";
                    break;

                case LogType.Assert:
                    prefix = "[Assert] ";
                    break;

                case LogType.Exception:
                    prefix = "[Exception] ";
                    break;
            }

            streamWriter.WriteLine(prefix + string.Format(format, args));
        }

        public void Dispose()
        {
            streamWriter.Close();
        }
    }

    static void InitLog()
    {
        if (Application.isPlaying && Application.installMode != ApplicationInstallMode.Editor)
        {
//            Debug.unityLogger.logHandler = new LogHandler();
        }
    }
}
