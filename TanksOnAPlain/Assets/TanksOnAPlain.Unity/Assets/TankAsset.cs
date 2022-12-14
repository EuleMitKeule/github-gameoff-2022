using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(TankAsset), fileName = nameof(TankAsset))]
    public class TankAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public LayerMask WallMask { get; set; }
    }
}