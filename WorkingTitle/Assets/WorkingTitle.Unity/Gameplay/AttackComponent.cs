using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Input;

namespace WorkingTitle.Unity.Gameplay
{
    public class AttackComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        GameObject WeaponPoint { get; set; }
        
        InputComponent InputComponent { get; set; }
        
        [OdinSerialize]
        GameObject ProjectilePrefab { get; set; }
        
        [OdinSerialize]
        float BulletSpeed { get; set; }
        
        [OdinSerialize]
        float Cooldown { get; set; }
        
        float LastAttackTime { get; set; }
        
        void Awake()
        {
            InputComponent = GetComponentInParent<InputComponent>();
        }

        void Update()
        {
            if (!InputComponent.InputPrimaryAttack) return;
            if (LastAttackTime + Cooldown > Time.time) return;

            LastAttackTime = Time.time;
            Attack();
        }

        void Attack()
        {
            var bullet = Instantiate(ProjectilePrefab, WeaponPoint.transform.position, transform.rotation);
            var bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = transform.up * BulletSpeed;
        }
    }
}