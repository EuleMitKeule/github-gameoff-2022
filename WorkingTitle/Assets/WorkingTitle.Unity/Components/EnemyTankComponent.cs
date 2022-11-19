using System;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.PowerUps;

namespace WorkingTitle.Unity.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyTankComponent : TankComponent
    {
        GameComponent GameComponent { get; set; }

        Rigidbody2D Rigidbody { get; set; }

        PowerUpConsumerComponent PowerUpConsumerComponent { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            
            Rigidbody = GetComponent<Rigidbody2D>();
            PowerUpConsumerComponent = GetComponent<PowerUpConsumerComponent>();
        }
        
        void Start()
        {
            GameComponent = GetComponentInParent<GameComponent>();
        }
        
        protected override void OnDeath(object sender, EventArgs e)
        {
            if (TankAsset is not EnemyTankAsset enemyTankAsset) return;
            
            Rigidbody.simulated = false;

            var skillPowerUpPrefab = enemyTankAsset.PowerUpAsset.Prefab;
            
            PowerUpConsumerComponent.DropPowerUp(skillPowerUpPrefab, GameComponent.GameAsset.DropChancePerPowerUp);
            PowerUpConsumerComponent.DropConsumedPowerUps(GameComponent.GameAsset.DropChancePerPowerUp);
            
            Destroy(gameObject);
        }
    }
}