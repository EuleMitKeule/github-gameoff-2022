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

namespace WorkingTitle.Unity.Components
{
    public class AttackComponent : SerializedMonoBehaviour, IResettable
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
        TankComponent TankComponent { get; set; }
        DifficultyComponent DifficultyComponent { get; set; }

        void Awake()
        {
            ProjectileComponents = new List<ProjectileComponent>();
            TankComponent = GetComponent<TankComponent>();
            InputComponent = GetComponent<InputComponent>();
            HealthComponent = GetComponent<HealthComponent>();
            
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
            ProjectileSpeed = AttackAsset.StartProjectileSpeed;
            AttackCooldown = AttackAsset.StartAttackCooldown;
            Damage = AttackAsset.StartDamage;
            Ricochets = AttackAsset.StartRicochets;
            LifeSteal = AttackAsset.StartLifeSteal;
            
            if (TankComponent.TankAsset is not EnemyTankAsset enemyTankAsset) return;
            
            //TODO: Skill scaling
        }

        void Attack()
        {
            var projectileObject = PoolComponent.Allocate(AttackAsset.ProjectilePrefab);
            var projectileComponent = projectileObject.GetComponent<ProjectileComponent>();
            var projectileRigidbody = projectileObject.GetComponent<Rigidbody2D>();
            var projectileTransform = projectileObject.GetComponent<Transform>();
            
            projectileComponent.Damage = Damage;
            projectileComponent.Ricochets = Ricochets;
            
            projectileTransform.position = WeaponPoint.transform.position;
            projectileTransform.rotation = WeaponPoint.transform.rotation;
            projectileRigidbody.velocity = TankComponent.TankCannon.transform.up * ProjectileSpeed;
            
            ProjectileComponents.Add(projectileComponent);
            
            projectileComponent.DamageInflicted += OnDamageInflicted;
        }

        void OnDamageInflicted(object sender, DamageInflictedEventArgs e)
        {
            var healAmount = e.Damage * LifeSteal / 100f;
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