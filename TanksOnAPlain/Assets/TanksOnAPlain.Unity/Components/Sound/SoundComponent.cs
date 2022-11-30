using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Sound
{
    public class SoundComponent : SerializedMonoBehaviour
    {
        [OdinSerialize] 
        Dictionary<SoundType, AudioSource> AudioSources { get; set; } = new();
        
        [OdinSerialize]
        Dictionary<SoundId, SoundType> SoundIdToSoundType { get; set; } = new();

        [OdinSerialize] 
        Dictionary<SoundId, List<AudioClip>> AudioClips { get; set; } = new();

        public void PlayClip(SoundId soundId)
        {
            if (!SoundIdToSoundType.ContainsKey(soundId)) return;
            if (!AudioClips.ContainsKey(soundId)) return;
            
            var soundType = SoundIdToSoundType[soundId];
            
            if (!AudioSources.ContainsKey(soundType)) return;
            
            var audioSource = AudioSources[soundType];
            var audioClips = AudioClips[soundId];
            
            var randomIndex = Random.Range(0, audioClips.Count);
            var audioClip = audioClips[randomIndex];

            audioSource.PlayOneShot(audioClip);
        }
    }
}
