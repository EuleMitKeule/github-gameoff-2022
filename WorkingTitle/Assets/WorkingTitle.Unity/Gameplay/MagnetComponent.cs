using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Gameplay.PowerUps;

namespace WorkingTitle.Unity.Gameplay
{
    public class MagnetComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [SuffixLabel("u")]
        public float Radius { get; set; }
        
        [OdinSerialize]
        [SuffixLabel("1/s^2")]
        float AccelerationPerDistance { get; set; }
        
        [OdinSerialize]
        [SuffixLabel("u/s^2")]
        float MaxAcceleration { get; set; }
        
        [OdinSerialize]
        LayerMask LayerMask { get; set; }
        
        Rigidbody2D Rigidbody { get; set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            var hits = Physics2D.CircleCastAll(transform.position, Radius, Vector2.zero, 0, LayerMask);

            foreach (var hit in hits)
            {
                var powerUpComponent = hit.collider.GetComponent<PowerUpComponent>();
                if (!powerUpComponent) continue;
                
                var rigidbody = hit.collider.GetComponent<Rigidbody2D>();
                var distance = (rigidbody.position - Rigidbody.position).magnitude;
                var acceleration = AccelerationPerDistance / distance;
                acceleration = Mathf.Max(acceleration, MaxAcceleration);
                var direction = (Rigidbody.position - rigidbody.position).normalized;
                rigidbody.AddForce(direction * acceleration, ForceMode2D.Force);
            }
        }
    }
}