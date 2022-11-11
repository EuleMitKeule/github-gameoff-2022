using System;
using Sirenix.OdinInspector;
using UnityEngine;
using WorkingTitle.Unity.Physics;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    public class PowerUpConsumerComponent : SerializedMonoBehaviour
    {
        PrimaryAttackComponent PrimaryAttackComponent { get; set; }
        TankMovementComponent TankMovementComponent { get; set; }
        MagnetComponent MagnetComponent { get; set; }

        public event EventHandler<PowerUpConsumedEventArgs> PowerUpConsumed;
        
        void Awake()
        {
            PrimaryAttackComponent = GetComponentInChildren<PrimaryAttackComponent>();
            TankMovementComponent = GetComponent<TankMovementComponent>();
            MagnetComponent = GetComponent<MagnetComponent>();
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            var powerUpComponent = other.GetComponent<PowerUpComponent>();

            if (!powerUpComponent) return;

            var powerUpAsset = powerUpComponent.PowerUpAsset;

            switch (powerUpAsset)
            {
                case RicochetPowerUpAsset ricochetPowerUpAsset:
                    PrimaryAttackComponent.Ricochets += ricochetPowerUpAsset.Ricochets;
                    break;
                case DamagePowerUpAsset damagePowerUpAsset:
                    PrimaryAttackComponent.Damage *= 1 + damagePowerUpAsset.DamagePercentageIncrease / 100;
                    break;
                case ProjectileSpeedPowerUpAsset projectileSpeedPowerUpAsset:
                    PrimaryAttackComponent.ProjectileSpeed *= 1 + projectileSpeedPowerUpAsset.ProjectileSpeedPercentageIncrease / 100;
                    break;
                case MovementSpeedPowerUpAsset movementSpeedPowerUpAsset:
                    TankMovementComponent.Speed *= 1 + movementSpeedPowerUpAsset.MovementSpeedPercentageIncrease / 100;
                    break;
                case AttackCooldownPowerUpAsset attackCooldownPowerUpAsset:
                    PrimaryAttackComponent.AttackCooldown *= 1 - attackCooldownPowerUpAsset.AttackCooldownPercentageDecrease / 100;
                    break;
                case LifeStealPowerUpAsset lifeStealPowerUpAsset:
                    if (PrimaryAttackComponent.LifeSteal > 0)
                    {
                        PrimaryAttackComponent.LifeSteal *= 1 + lifeStealPowerUpAsset.LifeStealPercentage / 100;
                    }
                    else
                    {
                        PrimaryAttackComponent.LifeSteal += lifeStealPowerUpAsset.LifeSteal;
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