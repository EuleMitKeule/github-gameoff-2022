using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components.Difficulty;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.Spawning
{
    [CreateAssetMenu(menuName = "Spawning/" + nameof(SpawnerAsset), fileName = nameof(SpawnerAsset))]
    public class SpawnerAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public float SpawnRadius { get; private set; }
        
        [OdinSerialize]
        public ScaledFloat Cooldown { get; private set; }
    }
}