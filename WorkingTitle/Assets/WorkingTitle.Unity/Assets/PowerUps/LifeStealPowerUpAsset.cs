using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(LifeStealPowerUpAsset), fileName = nameof(LifeStealPowerUpAsset))]
    public class LifeStealPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("*%")]
        public float LifeStealPercentage { get; set; }
        
        [OdinSerialize]
        [SuffixLabel("+%")]
        public float LifeSteal { get; set; }
    }
}