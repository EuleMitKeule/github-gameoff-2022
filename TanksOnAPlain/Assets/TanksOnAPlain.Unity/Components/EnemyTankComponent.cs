using System;
using TanksOnAPlain.Unity.Assets;
using TanksOnAPlain.Unity.Components.Pooling;
using TanksOnAPlain.Unity.Components.PowerUps;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyTankComponent : TankComponent, IDestroyable, IResettable
    {
        GameComponent GameComponent { get; set; }

        Rigidbody2D Rigidbody { get; set; }

        PowerUpConsumerComponent PowerUpConsumerComponent { get; set; }

        public event EventHandler Destroyed;
        
        protected override void Awake()
        {
            base.Awake();
            
            Rigidbody = GetComponent<Rigidbody2D>();
            PowerUpConsumerComponent = GetComponent<PowerUpConsumerComponent>();
            GameComponent = FindObjectOfType<GameComponent>();
        }
        
        protected override void OnDeath(object sender, EventArgs e)
        {
            if (TankAsset is not EnemyTankAsset enemyTankAsset) return;
            
            Rigidbody.simulated = false;

            var skillPowerUpPrefab = enemyTankAsset.PowerUpAsset.Prefab;
            
            PowerUpConsumerComponent.DropPowerUp(skillPowerUpPrefab, GameComponent.GameAsset.DropChancePerPowerUp);
            PowerUpConsumerComponent.DropConsumedPowerUps(GameComponent.GameAsset.DropChancePerPowerUp);
            
            Destroyed?.Invoke(this, EventArgs.Empty);
        }

        public void Reset()
        {
            Rigidbody.simulated = true;
        }
    }
}