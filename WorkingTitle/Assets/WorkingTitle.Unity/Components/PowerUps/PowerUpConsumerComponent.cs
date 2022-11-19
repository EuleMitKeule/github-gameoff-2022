using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WorkingTitle.Unity.Assets.PowerUps;
using WorkingTitle.Unity.Components.Physics;
using Random = UnityEngine.Random;

namespace WorkingTitle.Unity.Components.PowerUps
{
    public class PowerUpConsumerComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        Dictionary<PowerUpAsset, float> PowerUpsConsumed { get; set; } = new();
        
        AttackComponent AttackComponent { get; set; }
        TankMovementComponent TankMovementComponent { get; set; }
        MagnetComponent MagnetComponent { get; set; }
        GameComponent GameComponent { get; set; }
        public event EventHandler<PowerUpConsumedEventArgs> PowerUpConsumed;
        
        void Awake()
        {
            AttackComponent = GetComponentInChildren<AttackComponent>();
            TankMovementComponent = GetComponent<TankMovementComponent>();
            MagnetComponent = GetComponent<MagnetComponent>();

            PowerUpConsumed += OnPowerUpConsumed;
        }

        void Start()
        {
            GameComponent = GetComponentInParent<GameComponent>();
        }

        void OnPowerUpConsumed(object sender, PowerUpConsumedEventArgs e)
        {
            if (!PowerUpsConsumed.ContainsKey(e.PowerUpAsset)) PowerUpsConsumed[e.PowerUpAsset] = 0;
            PowerUpsConsumed[e.PowerUpAsset] += 1;
        }

        public void DropPowerUp(GameObject powerUpPrefab, float dropChance)
        {
            var random = Random.Range(0f, 1f);
            
            if (random > dropChance) return;

            var radius = Random.Range(0f, GameComponent.GameAsset.PowerUpDropRadius);
            var position = transform.position + (Vector3)Random.insideUnitCircle * radius;
            
            Instantiate(powerUpPrefab, position, Quaternion.identity);
        }
        
        public void DropConsumedPowerUps(float dropChancePerPowerup)
        {
            foreach (var (powerUpAsset, count) in PowerUpsConsumed)
            {
                for (int i = 0; i < count; i++)
                {
                    var powerUpPrefab = powerUpAsset.Prefab;
                    DropPowerUp(powerUpPrefab, dropChancePerPowerup);
                }
            }
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            var powerUpComponent = other.GetComponent<PowerUpComponent>();

            if (!powerUpComponent) return;

            var powerUpAsset = powerUpComponent.PowerUpAsset;

            switch (powerUpAsset)
            {
                case RicochetPowerUpAsset ricochetPowerUpAsset:
                    AttackComponent.Ricochets += ricochetPowerUpAsset.Ricochets;
                    break;
                case DamagePowerUpAsset damagePowerUpAsset:
                    AttackComponent.Damage *= 1 + damagePowerUpAsset.DamagePercentageIncrease / 100;
                    break;
                case ProjectileSpeedPowerUpAsset projectileSpeedPowerUpAsset:
                    AttackComponent.ProjectileSpeed *= 1 + projectileSpeedPowerUpAsset.ProjectileSpeedPercentageIncrease / 100;
                    break;
                case MovementSpeedPowerUpAsset movementSpeedPowerUpAsset:
                    TankMovementComponent.MovementSpeed *= 1 + movementSpeedPowerUpAsset.MovementSpeedPercentageIncrease / 100;
                    TankMovementComponent.RotationSpeed *= 1 + movementSpeedPowerUpAsset.RotationSpeedPercentageIncrease / 100;
                    break;
                case AttackCooldownPowerUpAsset attackCooldownPowerUpAsset:
                    AttackComponent.AttackCooldown *= 1 - attackCooldownPowerUpAsset.AttackCooldownPercentageDecrease / 100;
                    break;
                case LifeStealPowerUpAsset lifeStealPowerUpAsset:
                    if (AttackComponent.LifeSteal > 0)
                    {
                        AttackComponent.LifeSteal *= 1 + lifeStealPowerUpAsset.LifeStealPercentage / 100;
                    }
                    else
                    {
                        AttackComponent.LifeSteal += lifeStealPowerUpAsset.LifeSteal;
                    }
                    break;
                case MagnetPowerUpAsset magnetPowerUpAsset:
                    if (MagnetComponent.Radius > 0)
                    {
                        MagnetComponent.Radius *= 1 + magnetPowerUpAsset.RadiusPercentage / 100;
                    }
                    else
                    {
                        MagnetComponent.Radius += magnetPowerUpAsset.Radius;
                    }
                    break;
            }

            PowerUpConsumed?.Invoke(this, new PowerUpConsumedEventArgs(powerUpAsset));
            Destroy(other.gameObject);
        }
    }
}