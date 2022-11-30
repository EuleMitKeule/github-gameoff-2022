using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components.Difficulty;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(MagnetAsset), fileName = nameof(MagnetAsset))]
    public class MagnetAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        [SuffixLabel("u")]
        public ScaledFloat StartRadius { get; private set; }
        
        [OdinSerialize]
        [SuffixLabel("1/s^2")]
        public float AccelerationPerDistance { get; private set; }
        
        [OdinSerialize]
        [SuffixLabel("u/s^2")]
        public float MaxAcceleration { get; private set; }
        
        [OdinSerialize] 
        public LayerMask LayerMask { get; private set; }
    }
}