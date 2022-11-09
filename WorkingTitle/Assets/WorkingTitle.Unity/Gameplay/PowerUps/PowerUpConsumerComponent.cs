using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    public class PowerUpConsumerComponent : SerializedMonoBehaviour
    {
        PrimaryAttackComponent PrimaryAttackComponent { get; set; }

        public event EventHandler<PowerUpConsumedEventArgs> PowerUpConsumed;

        void Awake()
        {
            PrimaryAttackComponent = GetComponentInChildren<PrimaryAttackComponent>();
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            var powerUpComponent = other.GetComponent<PowerUpComponent>();

            if (!powerUpComponent) return;

            var powerUpAsset = powerUpComponent.PowerUpAsset;

            if (powerUpAsset is RicochetPowerUpAsset ricochetPowerUpAsset)
            {
                PrimaryAttackComponent.Ricochets += ricochetPowerUpAsset.Ricochets;
            }

            if (powerUpAsset is DamagePowerUpAsset damagePowerUpAsset)
            {
                PrimaryAttackComponent.Damage *= 1 + damagePowerUpAsset.DamagePercentageIncrease / 100;
            }

            PowerUpConsumed?.Invoke(this, new PowerUpConsumedEventArgs(powerUpAsset));
            Destroy(other.gameObject);
        }
    }
}