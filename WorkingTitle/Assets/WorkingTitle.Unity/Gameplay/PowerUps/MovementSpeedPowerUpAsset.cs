using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUpAsset/" + nameof(MovementSpeedPowerUpAsset), fileName = nameof(MovementSpeedPowerUpAsset))]
    public class MovementSpeedPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("%")]
        public float MovementSpeedPercentageIncrease { get; set; }
    }
}