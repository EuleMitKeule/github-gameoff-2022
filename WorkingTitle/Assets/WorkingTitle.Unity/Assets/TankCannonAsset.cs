using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(TankCannonAsset), fileName = nameof(TankCannonAsset))]
    public class TankCannonAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public float RotationSpeed { get; set; }
    }
}