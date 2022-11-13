using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(AiAsset), fileName = nameof(AiAsset))]
    public class AiAsset : SerializedScriptableObject
    {
        [TitleGroup("AI")]
        [OdinSerialize]
        public float TargetDirectionThreshold { get; private set; }

        [OdinSerialize]
        public float TargetDistanceThreshold { get; private set; }
        
        [OdinSerialize]
        public float AimOnTargetThreshold { get; private set; }
    }
}