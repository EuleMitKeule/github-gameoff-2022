using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(AttackAsset), fileName = nameof(AttackAsset))]
    public class AttackAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public GameObject ProjectilePrefab { get; private set; }
        
        [OdinSerialize]
        public float StartProjectileSpeed { get; private set; }
        
        [OdinSerialize]
        public float StartAttackCooldown { get; private set; }
        
        [OdinSerialize]
        public float StartDamage { get; private set; }
        
        [OdinSerialize]
        public int StartRicochets { get; private set; }
        
        [OdinSerialize]
        public float StartLifeSteal { get; private set; }

    }
}