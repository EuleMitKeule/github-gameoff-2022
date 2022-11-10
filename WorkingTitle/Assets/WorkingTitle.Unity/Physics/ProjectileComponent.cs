using System;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkingTitle.Unity.Gameplay;

namespace WorkingTitle.Unity.Physics
{
    public class ProjectileComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public float Damage { get; set; }
        
        [OdinSerialize]
        public float Ricochets { get; set; }
        
        [OdinSerialize]
        ContactFilter2D ContactFilter { get; set; }
        
        Rigidbody2D Rigidbody { get; set; }
        Collider2D Collider { get; set; }

        public event EventHandler<DamageInflictedEventArgs> DamageInflicted;

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
        }

        void FixedUpdate()
        {
            CheckCollision();
        }

        void CheckCollision()
        {
            var distance = Rigidbody.velocity.magnitude * Time.fixedDeltaTime;
            var hits = PerformCast(distance);
            
            if (hits.Length == 0) return;
            
            var hit = hits[0];
            HandleCollision(hit.collider.gameObject, hit.normal);
        }

        void HandleCollision(GameObject contactObject, Vector2 contactNormal)
        {
            HandleDamage(contactObject);
            HandleRicochet(contactNormal);
        }

        void HandleDamage(GameObject other)
        {
            var healthComponent = other.GetComponentInParent<HealthComponent>();
            if (healthComponent) healthComponent.ChangeHealth(-Damage);
            DamageInflicted?.Invoke(this, new DamageInflictedEventArgs(Damage));
        }

        void HandleRicochet(Vector2 contactNormal)
        {
            if (Ricochets <= 0)
            {
                Destroy(gameObject);
                return;
            }
            
            Ricochets -= 1;

            var reflectedVelocity = Vector2.Reflect(Rigidbody.velocity, contactNormal);
            
            Rigidbody.rotation = Vector2.SignedAngle(Vector2.up, reflectedVelocity);
            Rigidbody.velocity = reflectedVelocity;
        }

        RaycastHit2D[] PerformCast(float distance = 0)
        {
            var hits = new RaycastHit2D[10];
            var hitCount = Collider.Cast(
                Rigidbody.velocity,
                ContactFilter,
                hits,
                distance);
            Array.Resize(ref hits, hitCount);
            return hits;
        }

        void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Rigidbody) return;
            Debug.DrawLine(transform.position, transform.position + transform.up * Rigidbody.velocity.magnitude * Time.fixedDeltaTime, Color.red);
        }
#endif
    }
}