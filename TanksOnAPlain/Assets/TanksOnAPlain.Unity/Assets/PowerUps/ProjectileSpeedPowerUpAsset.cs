using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(ProjectileSpeedPowerUpAsset), fileName = nameof(ProjectileSpeedPowerUpAsset))]
    public class ProjectileSpeedPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("%")]
        public float ProjectileSpeedPercentageIncrease { get; set; }
    }
}