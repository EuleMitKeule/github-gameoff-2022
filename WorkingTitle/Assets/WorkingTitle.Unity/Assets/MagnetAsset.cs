using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(MagnetAsset), fileName = nameof(MagnetAsset))]
    public class MagnetAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        [SuffixLabel("u")]
        public float StartRadius { get; private set; }
        
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