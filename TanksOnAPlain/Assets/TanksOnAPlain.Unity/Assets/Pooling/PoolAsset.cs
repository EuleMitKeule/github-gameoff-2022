using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components.Pooling;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.Pooling
{
    [CreateAssetMenu(menuName = nameof(PoolAsset), fileName = nameof(PoolAsset))]
    public class PoolAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public List<Pool> Pools { get; private set; } = new();
    }
}