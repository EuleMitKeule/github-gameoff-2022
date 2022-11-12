using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Gameplay.PowerUps;

namespace WorkingTitle.Unity.Gameplay
{
    [CreateAssetMenu(fileName = nameof(SkillAsset), menuName = nameof(SkillAsset))]
    public class SkillAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public GameObject TankPrefab { get; set; }
        [OdinSerialize]
        TankAsset TankAsset { get; set; }
        
        [OdinSerialize]
        GameObject PowerUpPrefab { get; set; }
        [OdinSerialize]
        PowerUpAsset PowerUpAsset { get; set; }
    }
}