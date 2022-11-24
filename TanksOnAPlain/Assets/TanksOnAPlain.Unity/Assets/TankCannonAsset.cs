using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(TankCannonAsset), fileName = nameof(TankCannonAsset))]
    public class TankCannonAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public float RotationSpeed { get; set; }
    }
}