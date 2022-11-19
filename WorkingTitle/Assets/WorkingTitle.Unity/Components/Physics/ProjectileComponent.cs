using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.Health;
using WorkingTitle.Unity.Components.Pooling;

namespace WorkingTitle.Unity.Components.Physics
{
    public class ProjectileComponent : SerializedMonoBehaviour, IPoolable
    {
        [OdinSerialize]
        ProjectileAsset ProjectileAsset { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float Damage { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float Ricochets { get; set; }

        Rigidbody2D Rigidbody { get; set; }
        Collider2D Collider { get; set; }

        public event EventHandler<DamageInflictedEventArgs> DamageInflicted;
        public event EventHandler Destroyed;

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
        }

        void FixedUpdate()
        {
            CheckCollision();
        }

        public void Reset()
        {
            Damage = 0;
            Ricochets = 0;
            
            Rigidbody.transform.position = Vector3.zero;
            Rigidbody.transform.rotation = Quaternion.identity;
            Rigidbody.velocity = Vector2.zero;
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
            if (!healthComponent) return;
            healthComponent.ChangeHealth(-Damage);
            
            DamageInflicted?.Invoke(this, new DamageInflictedEventArgs(Damage));
        }

        void HandleRicochet(Vector2 contactNormal)
        {
            if (Ricochets <= 0)
            {
                Destroyed?.Invoke(this, EventArgs.Empty);
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
                ProjectileAsset.ContactFilter,
                hits,
                distance);
            Array.Resize(ref hits, hitCount);
            return hits;
        }

        void OnBecameInvisible()
        {
            Destroyed?.Invoke(this, EventArgs.Empty);
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