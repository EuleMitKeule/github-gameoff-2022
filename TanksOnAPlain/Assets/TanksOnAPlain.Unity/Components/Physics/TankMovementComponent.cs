using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets;
using TanksOnAPlain.Unity.Components.Input;
using TanksOnAPlain.Unity.Components.Pooling;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Physics
{
    [RequireComponent(typeof(TankComponent))]
    [RequireComponent(typeof(InputComponent))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class TankMovementComponent : SerializedMonoBehaviour, IResettable
    {
        [OdinSerialize]
        public TankMovementAsset TankMovementAsset { get; set; }

        public float MovementSpeed { get; set; }
        public float RotationSpeed { get; set; }
        
        TankComponent TankComponent { get; set; }
        
        InputComponent InputComponent { get; set; }
        
        Rigidbody2D Rigidbody { get; set; }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            TankComponent = GetComponent<TankComponent>();
            InputComponent = GetComponent<InputComponent>();
        }

        void FixedUpdate()
        {
            Move();
            Rotate();
        }
        
        public void Reset()
        {
            MovementSpeed = TankMovementAsset.MovementSpeed;
            RotationSpeed = TankMovementAsset.RotationSpeed;
        }

        void Move()
        {
            var speedBoostModifier = InputComponent.InputBoost ? TankMovementAsset.SpeedBoostModifier : 1;
            var direction = TankComponent.TankBody.transform.up;
            var velocity = direction * (speedBoostModifier * MovementSpeed * InputComponent.InputMovement * Time.fixedDeltaTime);
            
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
