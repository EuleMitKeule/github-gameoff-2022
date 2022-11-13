using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(HealthAsset), fileName = nameof(HealthAsset))]
    public class HealthAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public float StartHealth { get; private set; }
        
        [OdinSerialize]
        public float MaxHealth { get; private set; }
    }
}