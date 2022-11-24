using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(AttackCooldownPowerUpAsset), fileName = nameof(AttackCooldownPowerUpAsset))]
    public class AttackCooldownPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("%")]
        public float AttackCooldownPercentageDecrease { get; set; }
    }
}