using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components.Difficulty;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(TankMovementAsset), fileName = nameof(TankMovementAsset))]
    public class TankMovementAsset : SerializedScriptableObject
    {
        [TitleGroup("Physics")]
        [OdinSerialize]
        public ScaledFloat MovementSpeed { get; set; }
        
        [OdinSerialize]
        public ScaledFloat RotationSpeed { get; set; }
    }
}