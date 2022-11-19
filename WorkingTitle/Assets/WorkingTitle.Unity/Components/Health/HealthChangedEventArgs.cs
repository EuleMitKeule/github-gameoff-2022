using System;

namespace WorkingTitle.Unity.Components.Health
{
    public class HealthChangedEventArgs : EventArgs
    {
        public float PreviousHealth { get; }
        public float NewHealth { get; }
        public float HealthChange { get; }
        public float MaxHealth { get; }
    
        public HealthChangedEventArgs(float previousHealth, float newHealth, float healthChange, float maxHealth)
        {
            PreviousHealth = previousHealth;
            NewHealth = newHealth;
            HealthChange = healthChange;
            MaxHealth = maxHealth;
        }
    }
}