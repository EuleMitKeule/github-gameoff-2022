using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets.PowerUps;
using TanksOnAPlain.Unity.Components;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(EnemyTankAsset), fileName = nameof(EnemyTankAsset))]
    public class EnemyTankAsset : TankAsset
    {
        [OdinSerialize]
        public GameObject Prefab { get; private set; }
        
        [OdinSerialize]
        public PowerUpAsset PowerUpAsset { get; private set; }
        
        [OdinSerialize]
        public SkillType SkillType { get; private set; }
    }
}