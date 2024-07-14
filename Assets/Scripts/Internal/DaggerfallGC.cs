// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Pango (petchema@concept-micro.com)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace DaggerfallWorkshop
{
    public static class DaggerfallGC
    {
        // Min time between two unused assets collections
        private const float uuaThrottleDelay = 60f;

        private static float uuaTimer = Time.realtimeSinceStartup;
        private static AudioMixerGroup _defaultAudioMixerGroup;

        public static void ThrottledUnloadUnusedAssets(bool pruneCache = true)
        {
            Debug.Log("ThrottleUnloadUnusedAssets");
            if (pruneCache)
                DaggerfallUnity.Instance.PruneCache();
            if (Time.realtimeSinceStartup >= uuaTimer)
                ForcedUnloadUnusedAssets();
        }

        public static void ForcedUnloadUnusedAssets()
        {
            DaggerfallUnity.Instance.StartCoroutine(ForcedUnloadUnusedAssets_Coroutine());
        }

        private static IEnumerator ForcedUnloadUnusedAssets_Coroutine()
        {
            if (!_defaultAudioMixerGroup)
                _defaultAudioMixerGroup = Resources.Load<AudioMixer>("MainMixer").FindMatchingGroups("Master")[0];
            _defaultAudioMixerGroup.audioMixer.SetFloat("volume", -80);
            Debug.Log("ThrottleUnloadUnusedAssets: muted volume");
            yield return null;
            Debug.Log("ThrottleUnloadUnusedAssets: UnloadUnusedAssets");
            yield return Resources.UnloadUnusedAssets();
            Debug.Log("ThrottleUnloadUnusedAssets: unmuted volume");
            if (_defaultAudioMixerGroup)
                _defaultAudioMixerGroup.audioMixer.SetFloat("volume", 0);
            uuaTimer = Time.realtimeSinceStartup + uuaThrottleDelay;
        }
    }
}
