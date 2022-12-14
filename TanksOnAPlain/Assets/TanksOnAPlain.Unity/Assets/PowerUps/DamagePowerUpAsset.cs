using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(DamagePowerUpAsset), fileName = nameof(DamagePowerUpAsset))]
    public class DamagePowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("%")]
        public float DamagePercentageIncrease { get; set; }
    }
}