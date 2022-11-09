using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay
{
    public class HealthComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        float Health { get; set; }
        
        [OdinSerialize]
        float MaxHealth { get; set; }

        public event EventHandler Death;
        public event EventHandler<HealthChangedEventArgs> HealthChanged;

        public void ChangeHealth(float amount)
        {
            var previousHealth = Health;
            Health += amount;
            Health = Mathf.Clamp(Health, 0, MaxHealth);
            
            HealthChanged?.Invoke(this, new HealthChangedEventArgs(previousHealth, Health, amount));
            
            if (Health <= 0)
            {
                Death?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public class HealthChangedEventArgs : EventArgs
    {
        public float PreviousHealth { get; }
        public float NewHealth { get; }
        public float HealthChange { get; }
    
        public HealthChangedEventArgs(float previousHealth, float newHealth, float healthChange)
        {
            PreviousHealth = previousHealth;
            NewHealth = newHealth;
            HealthChange = healthChange;
        }
    }
}