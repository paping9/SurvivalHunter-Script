using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using AssetBundle;
using Utils;

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

    public partial class BGMManager : SingletonMB<BGMManager>
    {
        [Header("Components")]
        [SerializeField]
        private AudioModule[] _audios;

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
            get
            {
                return AudioModule.VolumeFactor;
            }
            set
            {
                var val = Mathf.Clamp01(value);
                if (AudioModule.VolumeFactor == val)
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

        public void Init()
        {
            //var option = Grm.LocalData.GetOption();
            ////Debug.Log(option.BGMOn + " : " + option.TotalSoundOn);
            //Mute = !option.TotalSoundOn || !option.BGMOn;
            //Volume = option.TotalSoundVolume * option.BGMVolume;
        }

        private void Awake()
        {
            _audios = new AudioModule[2];

            for (int i = 0; i < _audios.Length; ++i)
            {
                var comp_Audio = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                var newAudioModule = new AudioModule(comp_Audio);

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

            SystemLocator.Get<AddressableManager>().Load<AudioClip>(key, (clip) =>
            {
                CurrentKey = key;
                PlayBGMInternel(clip, fadeType, fadeTime, syncPrevClip, bResetSameClip);
            });
        }

        public void PlayBGM(AudioClip clip, FadeType fadeType, float fadeTime, bool syncPrevClip = false, bool bResetSameClip = false)
        {
            if (IgnoreChangeBgm)
                return;

            if (!bResetSameClip && (this.CurrentClip != null && clip != null)
                && (this.CurrentClip == clip || this.CurrentClip.name.CompareTo(clip.name) == 0))
                return;
            else
                this.CurrentClip = clip;

            PlayBGMInternel(clip, fadeType, fadeTime, syncPrevClip, bResetSameClip);
        }

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

                            System.Action outCallback = () =>
                            {
                                mainAudio.Stop();
                                mainAudio.Volume = 0f;

                                mainAudio.VolumeToDest(1f, fadeTime).Forget();
                                mainAudio.Play(clip, syncTime);
                            };

                            mainAudio.VolumeToDest(0f, fadeTime, outCallback).Forget();
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
