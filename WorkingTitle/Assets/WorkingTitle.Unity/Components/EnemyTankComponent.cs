using System;
using UnityEngine;
using WorkingTitle.Unity.Assets;

namespace WorkingTitle.Unity.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyTankComponent : TankComponent
    {
        GameComponent GameComponent { get; set; }

        Rigidbody2D Rigidbody { get; set; }

        protected override void Awake()
        {
            base.Awake();
            
            Rigidbody = GetComponent<Rigidbody2D>();
        }
        
        void Start()
        {
            GameComponent = GetComponentInParent<GameComponent>();
        }
        
        protected override void OnDeath(object sender, EventArgs e)
        {
            Rigidbody.simulated = false;
            
            DropPowerUp();
            
            Destroy(gameObject);
        }

        void DropPowerUp()
        {
            if (TankAsset is not EnemyTankAsset enemyTankAsset) return;
            var powerUpPrefab = GameComponent.GameAsset.SkillAssets[enemyTankAsset.SkillType].PowerUpPrefab;
            
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
    }
}