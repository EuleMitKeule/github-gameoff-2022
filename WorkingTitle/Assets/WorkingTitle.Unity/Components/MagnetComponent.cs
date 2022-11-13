using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.PowerUps;

namespace WorkingTitle.Unity.Components
{
    public class MagnetComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        MagnetAsset MagnetAsset { get; set; }
        
        public float Radius { get; set; }
        
        Rigidbody2D Rigidbody { get; set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Radius = MagnetAsset.StartRadius;
        }

        void FixedUpdate()
        {
            var hits = Physics2D.CircleCastAll(
                transform.position, 
                Radius, 
                Vector2.zero, 
                0, 
                MagnetAsset.LayerMask);

            foreach (var hit in hits)
            {
                var powerUpComponent = hit.collider.GetComponent<PowerUpComponent>();
                if (!powerUpComponent) continue;
                
                var hitRigidbody = hit.collider.GetComponent<Rigidbody2D>();
                var distance = (hitRigidbody.position - Rigidbody.position).magnitude;
                var acceleration = MagnetAsset.AccelerationPerDistance / distance;
                acceleration = Mathf.Max(acceleration, MagnetAsset.MaxAcceleration);
                var direction = (Rigidbody.position - hitRigidbody.position).normalized;
                hitRigidbody.AddForce(direction * acceleration, ForceMode2D.Force);
            }
        }
    }
}