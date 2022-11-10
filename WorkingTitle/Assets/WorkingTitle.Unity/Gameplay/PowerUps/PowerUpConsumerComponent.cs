using System;
using Sirenix.OdinInspector;
using UnityEngine;
using WorkingTitle.Unity.Physics;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    public class PowerUpConsumerComponent : SerializedMonoBehaviour
    {
        PrimaryAttackComponent PrimaryAttackComponent { get; set; }

        public event EventHandler<PowerUpConsumedEventArgs> PowerUpConsumed;

        TankMovementComponent TankMovementComponent { get; set; }
        
        void Awake()
        {
            PrimaryAttackComponent = GetComponentInChildren<PrimaryAttackComponent>();
            TankMovementComponent = GetComponent<TankMovementComponent>();
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
            }

            PowerUpConsumed?.Invoke(this, new PowerUpConsumedEventArgs(powerUpAsset));
            Destroy(other.gameObject);
        }
    }
}