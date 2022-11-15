using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.Health;
using WorkingTitle.Unity.Components.Input;
using WorkingTitle.Unity.Components.Physics;
using WorkingTitle.Unity.Components.Pooling;
using WorkingTitle.Unity.Components.Spawning;

namespace WorkingTitle.Unity.Components
{
    public class AttackComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        AttackAsset AttackAsset { get; set; }
        
        [OdinSerialize]
        public GameObject WeaponPoint { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float ProjectileSpeed { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float AttackCooldown { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float Damage { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public int Ricochets { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float LifeSteal { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        float LastAttackTime { get; set; }
        
        List<ProjectileComponent> ProjectileComponents { get; set; }
        
        InputComponent InputComponent { get; set; }
        
        HealthComponent HealthComponent { get; set; }
        
        PoolComponent PoolComponent { get; set; }

        void Awake()
        {
            ProjectileComponents = new List<ProjectileComponent>();
            
            InputComponent = GetComponentInParent<InputComponent>();
            HealthComponent = GetComponentInParent<HealthComponent>();
            PoolComponent = FindObjectOfType<PoolComponent>();
            
            HealthComponent.Death += OnDeath;
            
            ProjectileSpeed = AttackAsset.StartProjectileSpeed;
            AttackCooldown = AttackAsset.StartAttackCooldown;
            Damage = AttackAsset.StartDamage;
            Ricochets = AttackAsset.StartRicochets;
            LifeSteal = AttackAsset.StartLifeSteal;
        }

        void Update()
        {
            if (!InputComponent.InputPrimaryAttack) return;
            if (LastAttackTime + AttackCooldown > Time.time) return;

            LastAttackTime = Time.time;
            Attack();
        }

        void Attack()
        {
            var projectileComponent = PoolComponent.Allocate<ProjectileComponent>(AttackAsset.ProjectilePrefab);
            var projectileRigidbody = projectileComponent.GetComponent<Rigidbody2D>();
            
            projectileComponent.Damage = Damage;
            projectileComponent.Ricochets = Ricochets;
            
            projectileRigidbody.transform.position = WeaponPoint.transform.position;
            projectileRigidbody.transform.rotation = WeaponPoint.transform.rotation;
            projectileRigidbody.velocity = transform.up * ProjectileSpeed;
            
            ProjectileComponents.Add(projectileComponent);
            
            projectileComponent.DamageInflicted += OnDamageInflicted;
        }

        void OnDamageInflicted(object sender, DamageInflictedEventArgs e)
        {
            var healAmount = e.Damage * LifeSteal;
            HealthComponent.ChangeHealth(healAmount);
        }

        void OnDeath(object sender, EventArgs e)
        {
            foreach (var projectileComponent in ProjectileComponents)
            {
                if (!projectileComponent) continue;
                
                projectileComponent.DamageInflicted -= OnDamageInflicted;
            }
        }
    }
}