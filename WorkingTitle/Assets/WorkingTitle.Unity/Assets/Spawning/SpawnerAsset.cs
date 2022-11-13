using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets.PowerUps;

namespace WorkingTitle.Unity.Assets.Spawning
{
    [CreateAssetMenu(menuName = "Spawning/" + nameof(SpawnerAsset), fileName = nameof(SpawnerAsset))]
    public class SpawnerAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public float SpawnRadius { get; private set; }
        
        [OdinSerialize]
        [MinValue(0)]
        public float MaxSpawnCooldown { get; private set; }
        
        [OdinSerialize]
        [MinValue(0)]
        public float MinSpawnCooldown { get; private set; }
        
        [OdinSerialize]
        [MinValue(0)]
        public float SpawnCooldownDifficultyModifier { get; private set; }
    }
}