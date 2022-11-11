using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Gameplay.PowerUps;

namespace WorkingTitle.Unity.Gameplay
{
    [CreateAssetMenu(menuName = nameof(TankAsset), fileName = nameof(TankAsset))]
    public class TankAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public GameObject PowerUpPrefab { get; private set; }
    }
}