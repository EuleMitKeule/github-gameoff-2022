using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.Health;
using WorkingTitle.Unity.Components.Input;
using WorkingTitle.Unity.Components.Physics;

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
        
        List<GameObject> Projectiles { get; set; }
        
        InputComponent InputComponent { get; set; }
        
        HealthComponent HealthComponent { get; set; }

        void Awake()
        {
            Projectiles = new List<GameObject>();
            
            InputComponent = GetComponentInParent<InputComponent>();
            HealthComponent = GetComponentInParent<HealthComponent>();
            
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
            var projectile = Instantiate(
                AttackAsset.ProjectilePrefab, 
                WeaponPoint.transform.position, 
                transform.rotation);
            var projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
            var projectileComponent = projectile.GetComponent<ProjectileComponent>();
            
            projectileComponent.Damage = Damage;
            projectileComponent.Ricochets = Ricochets;
            projectileRigidbody.velocity = transform.up * ProjectileSpeed;
            
            Projectiles.Add(projectile);
            
            projectileComponent.DamageInflicted += OnDamageInflicted;
        }

        void OnDamageInflicted(object sender, DamageInflictedEventArgs e)
        {
            var healAmount = e.Damage * LifeSteal;
            HealthComponent.ChangeHealth(healAmount);
        }

        void OnDeath(object sender, EventArgs e)
        {
            foreach (var projectile in Projectiles)
            {
                if (!projectile) continue;
                
                var projectileComponent = projectile.GetComponent<ProjectileComponent>();
                if (!projectileComponent) continue;
                
                projectileComponent.DamageInflicted -= OnDamageInflicted;
            }
        }
    }
}