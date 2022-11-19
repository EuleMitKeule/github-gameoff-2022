using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets.PowerUps;
using WorkingTitle.Unity.Components;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(EnemyTankAsset), fileName = nameof(EnemyTankAsset))]
    public class EnemyTankAsset : TankAsset
    {
        [OdinSerialize]
        public GameObject Prefab { get; set; }
        
        [OdinSerialize]
        public PowerUpAsset PowerUpAsset { get; set; }
    }
}