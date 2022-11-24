using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.Spawning
{
    [CreateAssetMenu(menuName = "Spawning/" + nameof(SpawnTableAsset), fileName = nameof(SpawnTableAsset))]
    public class SpawnTableAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public Dictionary<SkillType, SpawnTableEntry> SpawnTable { get; set; } = new();
    }
}