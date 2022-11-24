using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(RicochetPowerUpAsset), fileName = nameof(RicochetPowerUpAsset))]
    public class RicochetPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        public int Ricochets { get; set; }
    }
}