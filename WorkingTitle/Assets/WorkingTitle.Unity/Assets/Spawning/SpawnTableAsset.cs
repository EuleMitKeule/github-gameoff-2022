using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Components;

namespace WorkingTitle.Unity.Assets.Spawning
{
    [CreateAssetMenu(menuName = "Spawning/" + nameof(SpawnTableAsset), fileName = nameof(SpawnTableAsset))]
    public class SpawnTableAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public Dictionary<SkillType, SpawnTableEntry> SpawnTable { get; set; } = new();
    }
}