using AssetBundle;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using VContainer;

namespace Sound
{
    public enum FadeType
    {
        None,
        FadeOut,
        FadeIn,
        FadeOutIn,
        CrossFade
    }

    public partial class BGMManager : MonoBehaviour, IBGMManager
    {
        [Header("Components")] 
        [SerializeField] private AudioModule[] _audios;

        private bool _mute = false;
        public bool Mute
        {
            get
            {
                return _mute;
            }
            set
            {
                if (_mute == value)
                    return;

                _mute = value;

                if (_audios != null)
                {
                    for (int i = 0; i < _audios.Length; ++i)
                    {
                        var audioModule = _audios[i];
                        if (audioModule == null)
                            continue;

                        audioModule.Mute = value;
                    }
                }
            }
        }
        public float Volume
        {
            get => AudioModule.VolumeFactor;
            set
            {
                var val = Mathf.Clamp01(value);
                if (Mathf.Approximately(AudioModule.VolumeFactor, val))
                    return;

                AudioModule.VolumeFactor = val;
                if (_audios != null)
                {
                    for (int i = 0; i < _audios.Length; ++i)
                    {
                        var audioModule = _audios[i];
                        if (audioModule == null)
                            continue;

                        audioModule.UpdateVolumeByFactor();
                    }
                }
            }
        }

        public AudioClip CurrentClip { get; private set; }
        public string CurrentKey { get; private set; }

        public bool IgnoreChangeBgm { get; set; } = false;

        private IAddressableManager _addressableManager;
        [Inject]
        public void Construct(IAddressableManager addressableManager)
        {
            _addressableManager = addressableManager;
        }

        /// <summary>
        /// Initializes the manager's audio modules by creating a two-element array and adding two AudioSource components to the GameObject, each wrapped in an AudioModule.
        /// </summary>
        private void Awake()
        {
            _audios = new AudioModule[2];

            for (int i = 0; i < _audios.Length; ++i)
            {
                var compAudio = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                var newAudioModule = new AudioModule(compAudio);

                _audios[i] = newAudioModule;
            }
        }



        public void PlayBGM(string key, FadeType fadeType, float fadeTime, bool syncPrevClip = false, bool bResetSameClip = false)
        {
            if (key == CurrentKey)
                return;

            if (IgnoreChangeBgm)
                return;

            if (string.IsNullOrEmpty(key))
            {
                //PlayBGM(null, fadeType, fadeTime, syncPrevClip, bResetSameClip);
                return;
            }

            _addressableManager.Load<AudioClip>(key, (clip) =>
            {
                CurrentKey = key;
                PlayBGMInternel(clip, fadeType, fadeTime, syncPrevClip, bResetSameClip);
            });
        }

        /// <summary>
        /// Starts playback of the specified background music clip using the given fade mode and duration.
        /// </summary>
        /// <param name="clip">The AudioClip to play as background music. Can be null to clear the current clip.</param>
        /// <param name="fadeType">The fade behavior to use when switching to the new clip.</param>
        /// <param name="fadeTime">Duration in seconds for the fade transition; values less than or equal to 0 result in an immediate change.</param>
        /// <param name="syncPrevClip">If true and a clip is currently playing, attempt to align the new clip's playback time with the current clip's playback position.</param>
        /// <param name="bResetSameClip">If true, restart playback even when the provided clip matches the currently playing clip; otherwise the method returns without changing playback.</param>
        /// <remarks>
        /// If changing BGM is globally ignored via IgnoreChangeBgm, this method returns without action. When playback proceeds, the manager's CurrentClip is updated to the provided clip and playback begins with the requested fade behavior.
        /// </remarks>
        public void PlayBGM(AudioClip clip, FadeType fadeType, float fadeTime, bool syncPrevClip = false, bool bResetSameClip = false)
        {
            if (IgnoreChangeBgm)
                return;

            if (!bResetSameClip && (this.CurrentClip != null && clip != null)
                && (this.CurrentClip == clip || String.Compare(this.CurrentClip.name, clip.name, StringComparison.Ordinal) == 0))
                return;
            else
                this.CurrentClip = clip;

            PlayBGMInternel(clip, fadeType, fadeTime, syncPrevClip, bResetSameClip);
        }

        /// <summary>
        /// Starts playback of the provided clip using the manager's two audio modules and applies the specified fade transition.
        /// </summary>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="fadeType">The fade mode to apply when switching to the clip.</param>
        /// <param name="fadeTime">The duration, in seconds, of the fade transition; values &lt;= 0 play immediately.</param>
        /// <param name="syncPrevClip">If true, align the new clip's playback time with the current main audio's playback time where applicable.</param>
        /// <param name="bResetSameClip">Indicates whether to force restarting the same clip when invoked (may be ignored by this internal routine).</param>
        private void PlayBGMInternel(AudioClip clip, FadeType fadeType, float fadeTime, bool syncPrevClip, bool bResetSameClip)
        {
            var mainAudio = _audios[0];
            var subAudio = _audios[1];

            if (fadeTime <= 0f)
            {
                PlayBGMInternal(clip);
                return;
            }

            switch (fadeType)
            {
                case FadeType.None:
                    {
                        PlayBGMInternal(clip);
                    }
                    break;

                case FadeType.FadeIn:
                    {
                        mainAudio.Stop();
                        subAudio.Stop();

                        float syncTime = 0f;
                        if (syncPrevClip && mainAudio.IsPlaying)
                            syncTime = mainAudio.PlaybackTime;

                        mainAudio.Volume = 0f;
                        mainAudio.VolumeToDest(1f, fadeTime).Forget();
                        mainAudio.Play(clip, syncTime);
                    }
                    break;

                case FadeType.FadeOut:
                    {
                        subAudio.Stop();

                        if (mainAudio.IsPlaying)
                        {
                            float syncTime = 0f;
                            if (syncPrevClip)
                                syncTime = mainAudio.PlaybackTime + fadeTime;

                            mainAudio.VolumeToDest(0f, fadeTime, () => PlayBGMInternal(clip, syncTime)).Forget();
                        }
                        else
                        {
                            PlayBGMInternal(clip);
                        }
                    }
                    break;

                case FadeType.FadeOutIn:
                    {
                        subAudio.Stop();

                        if (mainAudio.IsPlaying)
                        {
                            fadeTime *= 0.5f;

                            float syncTime = 0f;
                            if (syncPrevClip)
                                syncTime = mainAudio.PlaybackTime + fadeTime;

                            void OutCallback()
                            {
                                mainAudio.Stop();
                                mainAudio.Volume = 0f;

                                mainAudio.VolumeToDest(1f, fadeTime).Forget();
                                mainAudio.Play(clip, syncTime);
                            }

                            mainAudio.VolumeToDest(0f, fadeTime, OutCallback).Forget();
                        }
                        else
                        {
                            PlayBGM(clip, FadeType.FadeIn, fadeTime, false, true);
                        }
                    }
                    break;

                case FadeType.CrossFade:
                    {
                        if (mainAudio.IsPlaying)
                        {
                            float syncTime = 0f;
                            if (syncPrevClip)
                                syncTime = mainAudio.PlaybackTime;

                            var temp = _audios[0];
                            _audios[0] = _audios[1];
                            _audios[1] = temp;

                            mainAudio = _audios[0];
                            subAudio = _audios[1];

                            mainAudio.Volume = 0f;
                            mainAudio.VolumeToDest(1f, fadeTime).Forget();
                            subAudio.VolumeToDest(0f, fadeTime, () => subAudio.Stop()).Forget();

                            mainAudio.Play(clip, syncTime);
                        }
                        else
                        {
                            PlayBGM(clip, FadeType.FadeIn, fadeTime, false, true);
                        }
                    }
                    break;
            }
        }

        private void PlayBGMInternal(AudioClip clip, bool syncPrevClip = false)
        {
            this.CurrentClip = clip;

            var mainAudio = _audios[0];
            float syncTime = 0f;

            if (syncPrevClip && mainAudio.IsPlaying)
                syncTime = mainAudio.PlaybackTime;

            PlayBGMInternal(clip, syncTime);
        }

        private void PlayBGMInternal(AudioClip clip, float playbackTime)
        {
            this.CurrentClip = clip;

            var mainAudio = _audios[0];
            mainAudio.Play(clip, playbackTime);
            mainAudio.Volume = 1f;

            var subAudio = _audios[1];
            subAudio.Stop();
        }

        public void ChangeBgmTime(float time)
        {
            if (_audios[0].Clip == null)
                return;

            _audios[0].PlaybackTime = time;
        }
    }
}