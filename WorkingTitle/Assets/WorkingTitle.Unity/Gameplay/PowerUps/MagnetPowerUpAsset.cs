using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUpAsset/" + nameof(MagnetPowerUpAsset), fileName = nameof(MagnetPowerUpAsset))]
    public class MagnetPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        [SuffixLabel("u")]
        public float Radius { get; set; }
        
        [OdinSerialize]
        [SuffixLabel("%")]
        public float RadiusPercentage { get; set; }
    }
}