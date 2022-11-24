using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.Graphics
{
    [CreateAssetMenu(menuName = nameof(FloatingTextSpawnerAsset), fileName = nameof(FloatingTextSpawnerAsset))]
    public class FloatingTextSpawnerAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public GameObject FloatingTextPrefab { get; private set; }
    }
}