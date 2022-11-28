using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets;
using TanksOnAPlain.Unity.Components.Pooling;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Health
{
    public class HealthComponent : SerializedMonoBehaviour, IResettable
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
        HealthBarComponent HealthBarComponent { get; set; }
        
        [OdinSerialize]
        bool IsInvincible { get; set; }
        
        void Awake()
        {
            DifficultyComponent = FindObjectOfType<DifficultyComponent>();
            HealthBarComponent = GetComponentInChildren<HealthBarComponent>();
            
            if (HealthBarComponent)
                HealthChanged += HealthBarComponent.OnHealthChanged;
        }

        public void Reset()
        {
            var health = DifficultyComponent.GetScaledValueExp(
                HealthAsset.Health.MinValue, HealthAsset.Health.MaxValue, HealthAsset.Health.Time);

            MaxHealth = health;
            CurrentHealth = health;

            InvokeHealthChanged();
        }
        
        public void ChangeHealth(float amount)
        {
            var previousHealth = CurrentHealth;
            CurrentHealth += amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            
            HealthChanged?.Invoke(this, new HealthChangedEventArgs(previousHealth, CurrentHealth, amount, MaxHealth));
            
            if (!IsInvincible && CurrentHealth <= 0)
            {
                Death?.Invoke(this, EventArgs.Empty);
            }
        }

        public void InvokeHealthChanged()
        {
            HealthChanged?.Invoke(this, new HealthChangedEventArgs(CurrentHealth, CurrentHealth, 0, MaxHealth));
        }
    }
}