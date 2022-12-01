using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets;
using TanksOnAPlain.Unity.Components.Difficulty;
using TanksOnAPlain.Unity.Components.Health;
using TanksOnAPlain.Unity.Components.Input;
using TanksOnAPlain.Unity.Components.Physics;
using TanksOnAPlain.Unity.Components.Pooling;
using TanksOnAPlain.Unity.Components.Sound;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components
{
    public class AttackComponent : SerializedMonoBehaviour, IResettable
    {
        [OdinSerialize]
        AttackAsset AttackAsset { get; set; }
        
        [OdinSerialize]
        public GameObject WeaponPoint { get; private set; }
        
        [ShowInInspector]
        public float ProjectileSpeed { get; set; }
        
        [ShowInInspector]
        public float AttackCooldown { get; set; }
        
        [ShowInInspector]
        public float Damage { get; set; }
        
        [ShowInInspector]
        public int Ricochets { get; set; }
        
        [ShowInInspector]
        public int LifeSteal { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        float LastAttackTime { get; set; }
        
        List<ProjectileComponent> ProjectileComponents { get; set; }
        InputComponent InputComponent { get; set; }
        HealthComponent HealthComponent { get; set; }
        PoolComponent PoolComponent { get; set; }
        TankComponent TankComponent { get; set; }
        DifficultyComponent DifficultyComponent { get; set; }
        SoundComponent SoundComponent { get; set; }

        void Awake()
        {
            ProjectileComponents = new List<ProjectileComponent>();
            TankComponent = GetComponent<TankComponent>();
            InputComponent = GetComponent<InputComponent>();
            HealthComponent = GetComponent<HealthComponent>();
            SoundComponent = FindObjectOfType<SoundComponent>();
            
            PoolComponent = FindObjectOfType<PoolComponent>();
            DifficultyComponent = FindObjectOfType<DifficultyComponent>();
            
            HealthComponent.Death += OnDeath;
        }
        
        void Update()
        {
            if (!InputComponent.InputPrimaryAttack) return;
            if (LastAttackTime + AttackCooldown > Time.time) return;

            LastAttackTime = Time.time;
            Attack();
        }

        public void Reset()
        {
            ProjectileSpeed = DifficultyComponent.GetScaledValueExp(
                AttackAsset.ProjectileSpeed.StartValue,
                AttackAsset.ProjectileSpeed.EndValue,
                AttackAsset.ProjectileSpeed.Time);
            
            AttackCooldown = DifficultyComponent.GetScaledValueExp(
                AttackAsset.AttackCooldown.StartValue,
                AttackAsset.AttackCooldown.EndValue,
                AttackAsset.AttackCooldown.Time);
            
            Damage = DifficultyComponent.GetScaledValueExp(
                AttackAsset.Damage.StartValue,
                AttackAsset.Damage.EndValue,
                AttackAsset.Damage.Time);
            
            Ricochets = DifficultyComponent.GetScaledValueExp(
                AttackAsset.Ricochets.StartValue,
                AttackAsset.Ricochets.EndValue,
                AttackAsset.Ricochets.Time);
            
            LifeSteal = DifficultyComponent.GetScaledValueExp(
                AttackAsset.LifeSteal.StartValue,
                AttackAsset.LifeSteal.EndValue,
                AttackAsset.LifeSteal.Time);
        }

        void Attack()
        {
            var projectileObject = PoolComponent.Allocate(
                AttackAsset.ProjectilePrefab, 
                WeaponPoint.transform.position, 
                WeaponPoint.transform.rotation);
            var projectileComponent = projectileObject.GetComponent<ProjectileComponent>();
            var projectileRigidbody = projectileObject.GetComponent<Rigidbody2D>();
            
            projectileComponent.Damage = Damage;
            projectileComponent.Ricochets = Ricochets;
            projectileRigidbody.velocity = TankComponent.TankCannon.transform.up * ProjectileSpeed;
            
            ProjectileComponents.Add(projectileComponent);
            
            projectileComponent.DamageInflicted -= OnDamageInflicted;
            projectileComponent.DamageInflicted += OnDamageInflicted;
            
            SoundComponent.PlayClip(SoundId.ProjectileShot);
        }

        void OnDamageInflicted(object sender, DamageInflictedEventArgs e)
        {
            var healAmount = LifeSteal;
            if (healAmount <= 0) return;
            
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