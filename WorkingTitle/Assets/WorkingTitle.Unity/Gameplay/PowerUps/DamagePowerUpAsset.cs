using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUpAsset/" + nameof(DamagePowerUpAsset), fileName = nameof(DamagePowerUpAsset))]
    public class DamagePowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("%")]
        public float DamagePercentageIncrease { get; set; }
    }
}