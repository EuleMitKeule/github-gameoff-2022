using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components.Difficulty;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(AttackAsset), fileName = nameof(AttackAsset))]
    public class AttackAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public GameObject ProjectilePrefab { get; private set; }
        
        [OdinSerialize]
        public ScaledFloat ProjectileSpeed { get; private set; }
        
        [OdinSerialize]
        public ScaledFloat AttackCooldown { get; private set; }
        
        [OdinSerialize]
        public ScaledFloat Damage { get; private set; }
        
        [OdinSerialize]
        public ScaledInt Ricochets { get; private set; }
        
        [OdinSerialize]
        public ScaledInt LifeSteal { get; private set; }

    }
}