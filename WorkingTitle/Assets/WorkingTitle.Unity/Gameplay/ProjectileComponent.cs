using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay
{
    public class ProjectileComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        float Damage { get; set; }
        
        [OdinSerialize]
        LayerMask TargetLayer { get; set; }
        
        Rigidbody2D Rigidbody { get; set; }
        float Velocity { get; set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            CheckCollision();
        }

        void CheckCollision()
        {
            Velocity = Rigidbody.velocity.magnitude;
            
            var hit = Physics2D.Raycast(transform.position, transform.up, Velocity * Time.fixedDeltaTime, TargetLayer);
            if (!hit) return;
            
            var healthComponent = hit.collider.GetComponent<HealthComponent>();
            if (healthComponent) healthComponent.ChangeHealth(-Damage);

            Destroy(gameObject);
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            var healthComponent = other.gameObject.GetComponent<HealthComponent>();
            if (healthComponent) healthComponent.ChangeHealth(-Damage);

            Destroy(gameObject);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + transform.up * Velocity * Time.fixedDeltaTime, Color.red);
        }
#endif
    }
}