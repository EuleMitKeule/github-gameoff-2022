using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/" + nameof(LifeStealPowerUpAsset), fileName = nameof(LifeStealPowerUpAsset))]
    public class LifeStealPowerUpAsset : PowerUpAsset
    {
        [OdinSerialize] 
        [SuffixLabel("+")] 
        public int LifeSteal { get; set; } = 1;
    }
}