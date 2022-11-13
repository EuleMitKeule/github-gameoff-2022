using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets.PowerUps;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(fileName = nameof(SkillAsset), menuName = nameof(SkillAsset))]
    public class SkillAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public GameObject TankPrefab { get; set; }
        [OdinSerialize]
        TankAsset TankAsset { get; set; }
        
        [OdinSerialize] public GameObject PowerUpPrefab { get; set; }
        [OdinSerialize]
        PowerUpAsset PowerUpAsset { get; set; }
    }
}