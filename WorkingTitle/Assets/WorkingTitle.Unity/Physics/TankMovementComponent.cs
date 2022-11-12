using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Gameplay;
using WorkingTitle.Unity.Input;

namespace WorkingTitle.Unity.Physics
{
    [RequireComponent(typeof(TankComponent))]
    [RequireComponent(typeof(InputComponent))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class TankMovementComponent : SerializedMonoBehaviour
    {
        [TitleGroup("Physics")]
        [OdinSerialize]
        public float Speed { get; set; }
        
        [OdinSerialize]
        float SpeedBoostModifier { get; set; }
        
        [OdinSerialize]
        float RotationSpeed { get; set; }
        
        TankComponent TankComponent { get; set; }
        
        InputComponent InputComponent { get; set; }
        
        Rigidbody2D Rigidbody { get; set; }
        
        Animator Animator { get; set; }
        
        static readonly int IsMoving = Animator.StringToHash("isMoving");
        static readonly int IsRotatingRight = Animator.StringToHash("isRotatingRight");
        static readonly int IsRotatingLeft = Animator.StringToHash("isRotatingLeft");

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            TankComponent = GetComponent<TankComponent>();
            InputComponent = GetComponent<InputComponent>();
            Animator = GetComponent<Animator>();
        }

        void Update()
        {
            var isMoving = Mathf.Abs(InputComponent.InputMovement) != 0 ||
                           Mathf.Abs(InputComponent.InputRotation) != 0;
            Animator.SetBool(IsMoving, isMoving);
            
            var isRotatingLeft = InputComponent.InputRotation < 0;
            var isRotatingRight = InputComponent.InputRotation > 0;
            
            Animator.SetBool(IsRotatingLeft, isRotatingLeft);
            Animator.SetBool(IsRotatingRight, isRotatingRight);
        }

        void FixedUpdate()
        {
            Move();
            Rotate();
        }

        void Move()
        {
            var speedBoostModifier = InputComponent.InputBoost ? SpeedBoostModifier : 1;
            var direction = TankComponent.TankBody.transform.up;
            var velocity = direction * (speedBoostModifier * Speed * InputComponent.InputMovement * Time.fixedDeltaTime);
            
            Rigidbody.velocity = velocity;
        }

        void Rotate()
        {
            var zRotation = TankComponent.TankBody.transform.rotation.eulerAngles.z - InputComponent.InputRotation * RotationSpeed * Time.fixedDeltaTime;
            var rotation = Quaternion.Euler(0, 0, zRotation);
            
            TankComponent.TankBody.transform.rotation = rotation;
        }
    }
}
