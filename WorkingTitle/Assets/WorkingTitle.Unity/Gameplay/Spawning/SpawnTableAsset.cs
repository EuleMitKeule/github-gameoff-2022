using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.Spawning
{
    [CreateAssetMenu(menuName = nameof(SpawnTableAsset), fileName = nameof(SpawnTableAsset))]
    public class SpawnTableAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public Dictionary<SkillType, SpawnTableEntry> SpawnTable { get; set; } = new();
    }
}