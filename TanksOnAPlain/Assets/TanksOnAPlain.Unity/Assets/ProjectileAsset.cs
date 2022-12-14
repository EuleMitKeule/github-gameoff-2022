using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(ProjectileAsset), fileName = nameof(ProjectileAsset))]
    public class ProjectileAsset : SerializedScriptableObject
    {
        [OdinSerialize] 
        public ContactFilter2D ContactFilter { get; private set; }
    }
}