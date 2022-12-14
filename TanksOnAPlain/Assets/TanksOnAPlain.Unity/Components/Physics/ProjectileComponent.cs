using System;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets;
using TanksOnAPlain.Unity.Components.Health;
using TanksOnAPlain.Unity.Components.Map;
using TanksOnAPlain.Unity.Components.Pooling;
using TanksOnAPlain.Unity.Components.Sound;
using TanksOnAPlain.Unity.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TanksOnAPlain.Unity.Components.Physics
{
    public class ProjectileComponent : SerializedMonoBehaviour, IResettable, IDestroyable
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
        MapComponent MapComponent { get; set; }
        SoundComponent SoundComponent { get; set; }

        public event EventHandler<DamageInflictedEventArgs> DamageInflicted;
        public event EventHandler Destroyed;

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            MapComponent = FindObjectOfType<MapComponent>();
            SoundComponent = FindObjectOfType<SoundComponent>();
        }

        void FixedUpdate()
        {
            CheckInBounds();
            CheckCollision();
        }

        public void Reset()
        {
            Damage = 0;
            Ricochets = 0;
            
            Rigidbody.velocity = Vector2.zero;
        }

        void CheckInBounds()
        {
            if (!MapComponent.MapBounds.Contains(transform.position.ToCell()))
            {
                Destroyed?.Invoke(this, EventArgs.Empty);
            }
        }
        
        void CheckCollision()
        {
            var distance = Rigidbody.velocity.magnitude * Time.fixedDeltaTime;
            var hits = PerformCast(distance);
            
            if (hits.Length == 0) return;
            
            var hit = hits[0];
            HandleCollision(hit.collider.gameObject, hit.normal);
        }

        void HandleCollision(GameObject contactObject, Vector2 contactNormal, bool propagate = true)
        {
            HandleDamage(contactObject);
            HandleRicochet(contactNormal);
            
            var projectileComponent = contactObject.GetComponent<ProjectileComponent>();

            if (projectileComponent && propagate)
            {
                projectileComponent.HandleCollision(gameObject, -contactNormal, false);
            }
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
                SoundComponent.PlayClip(SoundId.ProjectileImpact);
                
                Destroyed?.Invoke(this, EventArgs.Empty);
                return;
            }
            
            SoundComponent.PlayClip(SoundId.ProjectileRicochet);
            
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

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Rigidbody) return;
            Debug.DrawLine(transform.position, transform.position + transform.up * Rigidbody.velocity.magnitude * Time.fixedDeltaTime, Color.red);
        }
#endif
    }
}