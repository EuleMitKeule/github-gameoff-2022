using System;
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
        
        InputComponent InputComponent { get; set; }
        
        HealthComponent HealthComponent { get; set; }

        void Awake()
        {
            InputComponent = GetComponentInParent<InputComponent>();
            HealthComponent = GetComponentInParent<HealthComponent>();
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
            var bullet = Instantiate(ProjectilePrefab, WeaponPoint.transform.position, transform.rotation);
            var bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            var projectileComponent = bullet.GetComponent<ProjectileComponent>();
            
            projectileComponent.Damage = Damage;
            projectileComponent.Ricochets = Ricochets;
            bulletRigidbody.velocity = transform.up * ProjectileSpeed;
            
            projectileComponent.DamageInflicted += OnDamageInflicted;
        }

        void OnDamageInflicted(object sender, DamageInflictedEventArgs e)
        {
            var healAmount = e.Damage * LifeSteal;
            HealthComponent.ChangeHealth(healAmount);
        }
    }
}