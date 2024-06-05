﻿using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class PlayerPrefsExtensions : PlayerPrefs
    {
        public static void SetBool(string key, bool value)
        {
            SetInt(key, value ? 1 : 0);
            Save();
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return GetInt(key, defaultValue ? 1 : 0) == 1 ? true : false;
        }

        public static void SetVector3(string key, Vector3 vector)
        {
            SetFloat($"{key}_x", vector.x);
            SetFloat($"{key}_y", vector.y);
            SetFloat($"{key}_z", vector.z);

            Save();
        }

        public static Vector3 GetVector3(string key, Vector3 defaultValue)
        {
            return new Vector3(GetFloat($"{key}_x", defaultValue.x),
                GetFloat($"{key}_y", defaultValue.y), GetFloat($"{key}_z", defaultValue.z));
        }

        public static void SetVector2(string key, Vector2 vector)
        {
            SetFloat($"{key}_x", vector.x);
            SetFloat($"{key}_y", vector.y);

            Save();
        }

        public static Vector2 GetVector2(string key, Vector2 defaultValue)
        {
            return new Vector2(GetFloat($"{key}_x", defaultValue.x),
                GetFloat($"{key}_y", defaultValue.y));
        }

        public static void DeleteVector3Key(string key)
        {
            DeleteKey($"{key}_x");
            DeleteKey($"{key}_y");
            DeleteKey($"{key}_z");
        }

        public static void DeleteVector2Key(string key)
        {
            DeleteKey($"{key}_x");
            DeleteKey($"{key}_y");
        }
    }
}