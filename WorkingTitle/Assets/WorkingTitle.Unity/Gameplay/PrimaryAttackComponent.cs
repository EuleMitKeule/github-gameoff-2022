using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Input;
using WorkingTitle.Unity.Physics;

namespace WorkingTitle.Unity.Gameplay
{
    public class PrimaryAttackComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        GameObject WeaponPoint { get; set; }
        
        [OdinSerialize]
        GameObject ProjectilePrefab { get; set; }
        
        [OdinSerialize]
        public float ProjectileSpeed { get; set; }
        
        [OdinSerialize]
        public float AttackCooldown { get; set; }
        
        [OdinSerialize]
        public float Damage { get; set; }
        
        [OdinSerialize]
        public int Ricochets { get; set; }
        
        [OdinSerialize]
        public float LifeSteal { get; set; }
        
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
            var projectile = Instantiate(ProjectilePrefab, WeaponPoint.transform.position, transform.rotation);
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