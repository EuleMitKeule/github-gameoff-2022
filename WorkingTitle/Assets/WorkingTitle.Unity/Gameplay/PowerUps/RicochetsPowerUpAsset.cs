using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    [CreateAssetMenu(menuName = nameof(RicochetsPowerUpAsset), fileName = nameof(RicochetsPowerUpAsset))]
    public class RicochetsPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize]
        public int Ricochets { get; set; }
    }
}