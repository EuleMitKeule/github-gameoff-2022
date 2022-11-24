using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(DifficultyAsset), fileName = nameof(DifficultyAsset))]
    public class DifficultyAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        [SuffixLabel("%/s")]
        public float DifficultyScaling { get; private set; }
    }
}