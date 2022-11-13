using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(ProjectileSpeedPowerUpAsset), fileName = nameof(ProjectileSpeedPowerUpAsset))]
    public class ProjectileSpeedPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("%")]
        public float ProjectileSpeedPercentageIncrease { get; set; }
    }
}