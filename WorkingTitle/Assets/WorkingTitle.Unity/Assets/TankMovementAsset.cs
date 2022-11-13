using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(TankMovementAsset), fileName = nameof(TankMovementAsset))]
    public class TankMovementAsset : SerializedScriptableObject
    {
        [TitleGroup("Physics")]
        [OdinSerialize]
        public float Speed { get; set; }
        
        [OdinSerialize] public float SpeedBoostModifier { get; set; }
        
        [OdinSerialize] public float RotationSpeed { get; set; }
    }
}