using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUpAsset/" + nameof(RicochetPowerUpAsset), fileName = nameof(RicochetPowerUpAsset))]
    public class RicochetPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        public int Ricochets { get; set; }
    }
}