using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.Graphics
{
    [CreateAssetMenu(menuName = nameof(TrackSpawnerAsset), fileName = nameof(TrackSpawnerAsset))]
    public class TrackSpawnerAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public GameObject TrackPrefab { get; private set; }
        
        [OdinSerialize]
        public float TrackDistance { get; private set; }
    }
}