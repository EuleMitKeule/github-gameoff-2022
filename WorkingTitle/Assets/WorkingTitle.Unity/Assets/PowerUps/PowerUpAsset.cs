using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets.PowerUps
{
    public abstract class PowerUpAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public GameObject Prefab { get; private set; }
    }
}