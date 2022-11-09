using Sirenix.OdinInspector;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    [RequireComponent(typeof(PrimaryAttackComponent))]
    public class PowerUpConsumerComponent : SerializedMonoBehaviour
    {
        PrimaryAttackComponent PrimaryAttackComponent { get; set; }

        void Awake()
        {
            PrimaryAttackComponent = GetComponent<PrimaryAttackComponent>();
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            var powerUpComponent = other.GetComponent<PowerUpComponent>();

            if (!powerUpComponent) return;

            var powerUpAsset = powerUpComponent.PowerUpAsset;

            if (powerUpAsset is RicochetsPowerUpAsset ricochetsPowerUpAsset)
            {
                PrimaryAttackComponent.Ricochets += ricochetsPowerUpAsset.Ricochets;
            }
        }
    }
}