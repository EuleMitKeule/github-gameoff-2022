using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components.Difficulty;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(HealthAsset), fileName = nameof(HealthAsset))]
    public class HealthAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public ScaledInt Health { get; set; }
    }
}