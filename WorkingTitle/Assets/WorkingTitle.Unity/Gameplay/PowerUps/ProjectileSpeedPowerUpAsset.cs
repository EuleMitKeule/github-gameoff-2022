using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUpAsset/" + nameof(ProjectileSpeedPowerUpAsset), fileName = nameof(ProjectileSpeedPowerUpAsset))]
    public class ProjectileSpeedPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("%")]
        public float ProjectileSpeedPercentageIncrease { get; set; }
    }
}