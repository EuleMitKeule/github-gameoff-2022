using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(GameAsset), fileName = nameof(GameAsset))]
    public class GameAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        [Optional]
        public GameObject MapPrefab { get; private set; }
        
        [OdinSerialize]
        [Optional]
        public GameObject PlayerPrefab { get; private set; }

        [OdinSerialize] 
        public Dictionary<SkillType, EnemyTankAsset> TankAssets { get; private set; } = new();
        
        [OdinSerialize]
        public float DropChancePerPowerUp { get; private set; }
        
        [OdinSerialize]
        public float PowerUpDropRadius { get; private set; }
    }
}