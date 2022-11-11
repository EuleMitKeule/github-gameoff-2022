using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay
{
    public class EnemyTankComponent : TankComponent
    {
        [TitleGroup("General")]
        [OdinSerialize]
        public TankAsset TankAsset { get; set; }
        
        protected override void OnDeath(object sender, EventArgs e)
        {
            var rigidbody = GetComponent<Rigidbody2D>();
            if (rigidbody) rigidbody.simulated = false;
            
            DropPowerUp();
            
            Destroy(gameObject);
        }

        void DropPowerUp()
        {
            Instantiate(TankAsset.PowerUpPrefab, transform.position, Quaternion.identity);
        }
    }
}