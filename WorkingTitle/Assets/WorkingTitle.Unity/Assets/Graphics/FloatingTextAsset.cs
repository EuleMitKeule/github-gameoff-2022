using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Components.Graphics;

namespace WorkingTitle.Unity.Assets.Graphics
{
    [CreateAssetMenu(menuName = nameof(FloatingTextAsset), fileName = nameof(FloatingTextAsset))]
    public class FloatingTextAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public float Speed { get; set; }
        
        [OdinSerialize]
        public float Duration { get; set; }
    }
}