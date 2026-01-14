using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Sound
{
    public partial class BGMManager
    {
        [System.Serializable]
        private class AudioModule
        {
            public static float VolumeFactor = 1f;

            [SerializeField]
            private AudioSource _audio;
            [SerializeField]
            private float _volume = 1f;
            public float Volume
            {
                get
                {
                    return _volume;
                }
                set
                {
                    _volume = value;

                    if (_audio != null)
                        _audio.volume = _volume * VolumeFactor;
                }
            }

            public bool IsPlaying
            {
                get
                {
                    if (_audio == null)
                        return false;

                    return _audio.isPlaying;
                }
            }

            public AudioClip Clip
            {
                get
                {
                    if (_audio == null)
                        return null;

                    return _audio.clip;
                }
                set
                {
                    if (_audio == null)
                        return;

                    _audio.clip = value;
                }
            }

            public bool Mute
            {
                get
                {
                    return _audio.mute;
                }
                set
                {
                    _audio.mute = value;
                }
            }

            public float PlaybackTime
            {
                get
                {
                    if (_audio.clip == null || !_audio.isPlaying)
                        return 0f;

                    return _audio.time;
                }
                set
                {
                    if (_audio.clip == null)
                        return;

                    _audio.time = Mathf.Clamp(value, 0f, _audio.clip.length);
                }
            }



            /// <summary>
            /// Initializes a new AudioModule bound to the specified AudioSource and enables looping on that source.
            /// </summary>
            /// <param name="audio">The AudioSource instance that this module will control.</param>
            public AudioModule(AudioSource audio)
            {
                _audio = audio;
                _audio.loop = true;
            }

            /// <summary>
            /// Smoothly transitions the module's volume to the specified target over the given duration.
            /// </summary>
            /// <param name="volume">Target volume in the range [0, 1].</param>
            /// <param name="time">Transition duration in seconds.</param>
            /// <param name="endCallback">Optional callback invoked after the transition completes or immediately if no audio source is available or the current volume already equals the target.</param>
            /// <returns>A UniTask that completes after the volume transition finishes and the optional callback has been invoked.</returns>
            public async UniTask VolumeToDest(float volume, float time, System.Action endCallback = null)
            {
                if (_audio == null)
                {
                    endCallback?.Invoke();
                    return;
                }

                float srcVolume = Volume;
                float dstVolume = Mathf.Clamp01(volume);

                if (Mathf.Approximately(srcVolume, dstVolume))
                {
                    endCallback?.Invoke();

                    return;
                }

                float timeInverse = 1f / time;
                float duration = 0f;

                while (true)
                {
                    await UniTask.WaitForFixedUpdate();

                    duration += Time.fixedUnscaledDeltaTime;
                    if (duration >= time)
                        break;

                    var factor = duration * timeInverse;
                    Volume = Mathf.Lerp(srcVolume, dstVolume, factor);
                }

                Volume = volume;

                if (endCallback != null)
                    endCallback.Invoke();
            }

            /// <summary>
            /// Applies the module's effective volume factor to the underlying AudioSource's volume.
            /// </summary>
            /// <remarks>
            /// If no AudioSource is assigned, the method does nothing.
            /// </remarks>
            public void UpdateVolumeByFactor()
            {
                if (_audio != null)
                    _audio.volume = _volume * VolumeFactor;
            }

            public bool Play(AudioClip clip)
            {
                if (_audio == null)
                    return false;

                Clip = clip;
                if (Clip == null)
                {
                    _audio.Stop();
                    return false;
                }

                _audio.time = 0f;
                _audio.Play();
                return true;
            }

            public void Play(AudioClip clip, float playbackTime)
            {
                if (Play(clip))
                    PlaybackTime = Mathf.Clamp(playbackTime, 0f, clip.length);
            }

            public void Stop()
            {
                if (_audio == null)
                    return;

                Clip = null;
                _audio.Stop();
            }
        }

    }
}