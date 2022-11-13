using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(MagnetPowerUpAsset), fileName = nameof(MagnetPowerUpAsset))]
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