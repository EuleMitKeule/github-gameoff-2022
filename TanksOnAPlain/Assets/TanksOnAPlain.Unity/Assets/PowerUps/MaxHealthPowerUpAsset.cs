using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(MaxHealthPowerUpAsset), fileName = nameof(MaxHealthPowerUpAsset))]
    public class MaxHealthPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        public int MaxHealthIncrease { get; set; }
    }
}