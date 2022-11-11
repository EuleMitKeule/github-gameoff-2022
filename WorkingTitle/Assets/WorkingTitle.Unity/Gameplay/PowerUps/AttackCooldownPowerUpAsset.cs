using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUpAsset/" + nameof(AttackCooldownPowerUpAsset), fileName = nameof(AttackCooldownPowerUpAsset))]
    public class AttackCooldownPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("%")]
        public float AttackCooldownPercentageDecrease { get; set; }
    }
}