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
        float Damage { get; set; }
        
        [OdinSerialize]
        float Ricochets { get; set; }
        
        [OdinSerialize]
        ContactFilter2D ContactFilter { get; set; }
        
        Rigidbody2D Rigidbody { get; set; }
        
        Collider2D Collider { get; set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
        }

        void FixedUpdate()
        {
            CheckCollision();
        }

        void Update()
        {
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                //clear unity console
                var assembly = Assembly.GetAssembly(typeof(UnityEditor.ActiveEditorTracker));
                var type = assembly.GetType("UnityEditor.LogEntries");
                var method = type.GetMethod("Clear");
                method.Invoke(new object(), null);
                Destroy(gameObject);
            }
        }

        void CheckCollision()
        {
            var distance = Rigidbody.velocity.magnitude * Time.fixedDeltaTime;
            var hits = PerformCast(distance);
            
            if (hits.Length == 0) return;
            
            var hit = hits[0];
            HandleCollision(hit.collider.gameObject, hit.point, hit.normal);
        }

        // void Check2ndCollision()
        // {
        //     var distance = Rigidbody.velocity.magnitude * Time.fixedDeltaTime;
        //     var hits = new RaycastHit2D[1];
        //     var hitCount = Collider.Cast(
        //         Rigidbody.velocity,
        //         ContactFilter,
        //         hits,
        //         distance);
        //
        //     Debug.Log($"hitCount: {hits.Length}");
        //     if (hitCount <= 1)
        //     {
        //         Rigidbody.position += Rigidbody.velocity * Time.fixedDeltaTime;
        //         return;
        //     }
        //     var hit = hits[1];
        //     
        //     HandleCollision(hit.collider.gameObject, hit.point, hit.normal);
        // }

        void HandleCollision(GameObject contactObject, Vector2 contactPosition, Vector2 contactNormal)
        {
            HandleDamage(contactObject);
            HandleRicochet(contactPosition, contactNormal);
        }

        void HandleDamage(GameObject other)
        {
            var healthComponent = other.GetComponent<HealthComponent>();
            if (healthComponent) healthComponent.ChangeHealth(-Damage);
        }

        void HandleRicochet(Vector2 contactPosition, Vector2 contactNormal)
        {
            if (Ricochets <= 0)
            {
                Destroy(gameObject);
                return;
            }
            
            Ricochets -= 1;

            // if (contactNormal == -Rigidbody.velocity.normalized)
            // {
            //     Debug.LogWarning("Projectile was inside object");
            //     Destroy(gameObject);
            //     return;
            // }
            
            // var correctNormalSign = Vector2.Dot(contactNormal, Rigidbody.velocity) > 0 ? -1 : 1;
            // contactNormal *= correctNormalSign;
            // if (correctNormalSign < 0)
            // {
            //     Debug.Log("Projectile collided from inside object");
            // }

            var reflectedVelocity = Vector2.Reflect(Rigidbody.velocity, contactNormal);
            
            // var collisionAngle = Vector2.SignedAngle(-Rigidbody.velocity, contactNormal);
            // var reflectionDirection = Quaternion.Euler(0, 0, collisionAngle) * contactNormal * Rigidbody.velocity.magnitude;
            
            //Rigidbody.position = (Vector3)contactPosition + reflectionDirection.normalized * (Rigidbody.velocity.magnitude * Time.fixedDeltaTime);
            // Rigidbody.position = contactPosition;
            Rigidbody.rotation = Vector2.SignedAngle(Vector2.up, reflectedVelocity);
            Rigidbody.velocity = reflectedVelocity;

            //Debug.Log($"contactPosition: {contactPosition}\tnormal: {contactNormal}\nposition: {transform.position}\tvelocity: {Rigidbody.velocity}");
            Debug.DrawRay(contactPosition, contactNormal, Color.cyan, 1);
            
            // Check2ndCollision();
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

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!Rigidbody) return;
            Debug.DrawLine(transform.position, transform.position + transform.up * Rigidbody.velocity.magnitude * Time.fixedDeltaTime, Color.red);
        }
#endif
    }
}