using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Components;

namespace WorkingTitle.Unity.Assets
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
        public Dictionary<SkillType, SkillAsset> SkillAssets { get; private set; } = new();
    }
}