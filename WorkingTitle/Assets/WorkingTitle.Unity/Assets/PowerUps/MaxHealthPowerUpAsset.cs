using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(MaxHealthPowerUpAsset), fileName = nameof(MaxHealthPowerUpAsset))]
    public class MaxHealthPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        public int MaxHealthIncrease { get; set; }
    }
}