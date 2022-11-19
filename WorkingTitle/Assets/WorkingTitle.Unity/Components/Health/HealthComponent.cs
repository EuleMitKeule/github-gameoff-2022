using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;

namespace WorkingTitle.Unity.Components.Health
{
    public class HealthComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        HealthAsset HealthAsset { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float MaxHealth { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        float CurrentHealth { get; set; }
        
        public event EventHandler Death;
        public event EventHandler<HealthChangedEventArgs> HealthChanged;

        DifficultyComponent DifficultyComponent { get; set; }
        
        void Awake()
        {
            MaxHealth = HealthAsset.MaxHealth;
            CurrentHealth = HealthAsset.StartHealth;
        }

        void Start()
        {
            DifficultyComponent = GetComponentInParent<DifficultyComponent>();
        }
        
        public void ChangeHealth(float amount)
        {
            var previousHealth = CurrentHealth;
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            
            HealthChanged?.Invoke(this, new HealthChangedEventArgs(previousHealth, CurrentHealth, amount));
            
            if (CurrentHealth <= 0)
            {
                Death?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}