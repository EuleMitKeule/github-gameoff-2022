using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(MovementSpeedPowerUpAsset), fileName = nameof(MovementSpeedPowerUpAsset))]
    public class MovementSpeedPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("%")]
        public float MovementSpeedPercentageIncrease { get; set; }
        
        [OdinSerialize]
        [SuffixLabel("%")]
        public float RotationSpeedPercentageIncrease { get; set; }
    }
}