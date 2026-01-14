using UnityEngine;

namespace Sound
{
    public interface IBGMManager
    {
        bool Mute { get; set; }
        float Volume { get; set; }
        AudioClip CurrentClip { get; }
        string CurrentKey { get; }
        bool IgnoreChangeBgm { get; set; }
        void PlayBGM(string key, FadeType fadeType, float fadeTime, bool syncPrevClip = false, bool bResetSameClip = false);
        void PlayBGM(AudioClip clip, FadeType fadeType, float fadeTime, bool syncPrevClip = false, bool bResetSameClip = false);
        void ChangeBgmTime(float time);
    }
}