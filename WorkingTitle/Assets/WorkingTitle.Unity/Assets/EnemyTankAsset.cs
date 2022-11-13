using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Components;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(EnemyTankAsset), fileName = nameof(EnemyTankAsset))]
    public class EnemyTankAsset : TankAsset
    {
        [OdinSerialize]
        public SkillType SkillType { get; private set; }
    }
}