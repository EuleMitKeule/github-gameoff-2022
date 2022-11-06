using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay
{
    public class HealthComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        float Health { get; set; }
        
        [OdinSerialize]
        float MaxHealth { get; set; }

        public void ChangeHealth(float amount)
        {
            Health += amount;
            Health = Mathf.Clamp(Health, 0, MaxHealth);
        }
    }
}