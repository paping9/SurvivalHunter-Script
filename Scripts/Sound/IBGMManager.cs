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
        /// <summary>
/// Plays background music identified by the specified key using the given fade settings.
/// </summary>
/// <param name="key">Identifier of the BGM to play.</param>
/// <param name="fadeType">Type of fade transition to apply when changing tracks.</param>
/// <param name="fadeTime">Duration of the fade transition in seconds.</param>
/// <param name="syncPrevClip">If true, synchronize the new track's playback position with the previous clip.</param>
/// <param name="bResetSameClip">If true, restart playback when the requested clip is already playing; otherwise do nothing.</param>
void PlayBGM(string key, FadeType fadeType, float fadeTime, bool syncPrevClip = false, bool bResetSameClip = false);
        /// <summary>
/// Plays the specified AudioClip as background music using the provided fade settings.
/// </summary>
/// <param name="clip">The AudioClip to play as background music.</param>
/// <param name="fadeType">The fade transition type to apply when switching to this clip.</param>
/// <param name="fadeTime">Fade duration in seconds.</param>
/// <param name="syncPrevClip">If true, synchronize this clip's playback position with the previous clip's position.</param>
/// <param name="bResetSameClip">If true and the requested clip is already playing, reset its playback position instead of leaving it unchanged.</param>
void PlayBGM(AudioClip clip, FadeType fadeType, float fadeTime, bool syncPrevClip = false, bool bResetSameClip = false);
        /// <summary>
/// Sets the playback position of the currently playing background music to the specified time in seconds.
/// </summary>
/// <param name="time">Target playback time in seconds.</param>
void ChangeBgmTime(float time);
    }
}