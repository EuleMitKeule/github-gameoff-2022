using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components.Rendering;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(CameraAsset), fileName = nameof(CameraAsset))]
    public class CameraAsset : SerializedScriptableObject
    {
        [OdinSerialize] 
        public float MaxDistance { get; private set; }
    
        [OdinSerialize] 
        public float MouseFollowDamping { get; private set; }
    
        [OdinSerialize] 
        public CameraCorrectionMode CameraCorrectionMode { get; private set; }   
    }
}