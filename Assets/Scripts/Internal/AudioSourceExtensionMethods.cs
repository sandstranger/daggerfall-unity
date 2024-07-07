using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop;

namespace UnityEngine
{
    public static class AudioSourceExtensionMethods
    {
        private const float audioClipMaxDelay = 0.150f; //give up if sound takes longer to load
        private const float audioLoopMaxDelay = 2.0f; // don't care as much about loops accuracy

        private static Dictionary<int, AudioClip> waitingClips = new Dictionary<int, AudioClip>();

        public static void PlayWhenReady(this AudioSource audioSource, AudioClip audioClip, float volumeScale, int id = -1)
        {
            DaggerfallUnity.Instance.StartCoroutine(WhenClipReadyCoroutine(audioClip, audioLoopMaxDelay,
                clip =>
                {
                    audioSource.clip = clip;
                    audioSource.volume = volumeScale * DaggerfallUnity.Settings.SoundVolume;
                    audioSource.Play();
                }, id));
        }

        public static void PlayOneShotWhenReady(this AudioSource audioSource, AudioClip audioClip, float volumeScale, int id = -1)
        {
            DaggerfallUnity.Instance.StartCoroutine(WhenClipReadyCoroutine(audioClip, audioClipMaxDelay,
                clip =>
                {
                    audioSource.volume = volumeScale * DaggerfallUnity.Settings.SoundVolume;
                    audioSource.PlayOneShot(clip);
                }, id));
        }

        public static void PlayClipAtPointWhenReady(AudioClip audioClip, Vector3 position, float volumeScale, int id = -1)
        {
            DaggerfallUnity.Instance.StartCoroutine(WhenClipReadyCoroutine(audioClip, audioClipMaxDelay,
                clip => AudioSource.PlayClipAtPoint(clip, position, volumeScale * DaggerfallUnity.Settings.SoundVolume), id));
        }

        private static IEnumerator WhenClipReadyCoroutine(AudioClip audioClip, float maxDelay, Action<AudioClip> clipHandler, int id = -1)
        {
            if (id >= 0 && waitingClips.ContainsKey(id))
                yield break;
            if (id >= 0)
                waitingClips[id] = audioClip;
            float loadWaitTimer = 0f;
            while (audioClip.loadState == AudioDataLoadState.Unloaded ||
                   audioClip.loadState == AudioDataLoadState.Loading)
            {
                loadWaitTimer += Time.unscaledDeltaTime;
                if (loadWaitTimer > maxDelay)
                {
                    if (id >= 0)
                        waitingClips.Remove(id);
                    yield break;

                }
                yield return null;
            }
            if (id >= 0)
                waitingClips.Remove(id);
            clipHandler(audioClip);
        }
    }
}
