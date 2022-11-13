using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.Input;

namespace WorkingTitle.Unity.Components.Physics
{
    [RequireComponent(typeof(TankComponent))]
    [RequireComponent(typeof(InputComponent))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class TankMovementComponent : SerializedMonoBehaviour
    {
        [OdinSerialize] public TankMovementAsset TankMovementAsset { get; set; }
        
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
            var speedBoostModifier = InputComponent.InputBoost ? TankMovementAsset.SpeedBoostModifier : 1;
            var direction = TankComponent.TankBody.transform.up;
            var velocity = direction * (speedBoostModifier * TankMovementAsset.Speed * InputComponent.InputMovement * Time.fixedDeltaTime);
            
            Rigidbody.velocity = velocity;
        }

        void Rotate()
        {
            var zRotation = TankComponent.TankBody.transform.rotation.eulerAngles.z - InputComponent.InputRotation * TankMovementAsset.RotationSpeed * Time.fixedDeltaTime;
            var rotation = Quaternion.Euler(0, 0, zRotation);
            
            TankComponent.TankBody.transform.rotation = rotation;
        }
    }
}
