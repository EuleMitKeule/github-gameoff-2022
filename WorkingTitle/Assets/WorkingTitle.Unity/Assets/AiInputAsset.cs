using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(AiInputAsset), fileName = nameof(AiInputAsset))]
    public class AiInputAsset : SerializedScriptableObject
    {
        [OdinSerialize] public float AimRotationThreshold { get; private set; }
    }
}