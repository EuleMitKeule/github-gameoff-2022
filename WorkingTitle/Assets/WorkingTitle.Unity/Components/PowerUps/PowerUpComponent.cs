using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets.PowerUps;
using WorkingTitle.Unity.Components.Pooling;

namespace WorkingTitle.Unity.Components.PowerUps
{
    public class PowerUpComponent : SerializedMonoBehaviour, IDestroyable
    {
        [OdinSerialize] 
        public PowerUpAsset PowerUpAsset { get; set; }
        public event EventHandler Destroyed;
        
        void OnTriggerEnter2D(Collider2D other)
        {
            var powerUpConsumerComponent = other.GetComponentInParent<PowerUpConsumerComponent>();

            if (!powerUpConsumerComponent) return;

            powerUpConsumerComponent.Consume(PowerUpAsset);
            
            Destroyed?.Invoke(this, EventArgs.Empty);
        }
    }
}