using System.Collections;
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



            public AudioModule(AudioSource audio)
            {
                _audio = audio;
                _audio.loop = true;
            }

            public async UniTask VolumeToDest(float volume, float time, System.Action _endCallback = null)
            {
                if (_audio == null)
                {
                    _endCallback?.Invoke();
                    return;
                }

                float srcVolume = Volume;
                float dstVolume = Mathf.Clamp01(volume);

                if (srcVolume == dstVolume)
                {
                    _endCallback?.Invoke();

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

                if (_endCallback != null)
                    _endCallback.Invoke();
            }

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