using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.Graphics
{
    [CreateAssetMenu(menuName = nameof(TrackAsset), fileName = nameof(TrackAsset))]
    public class TrackAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public float LifeTime { get; set; }
        
        [OdinSerialize]
        public float StartAlpha { get; set; }
        
        [OdinSerialize]
        public GameObject TrackPrefab { get; set; }
    }
}