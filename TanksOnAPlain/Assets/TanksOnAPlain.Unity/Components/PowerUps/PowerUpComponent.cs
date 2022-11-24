using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets.PowerUps;
using TanksOnAPlain.Unity.Components.Pooling;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.PowerUps
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