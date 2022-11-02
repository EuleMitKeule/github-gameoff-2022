using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Input;

namespace WorkingTitle.Unity.Physics
{
    public class TankMovementComponent : SerializedMonoBehaviour
    {
        [TitleGroup("General")]
        [OdinSerialize]
        [ValueDropdown("ChildObjects")]
        protected GameObject TankBody { get; private set; }
        
        [UsedImplicitly]
        IEnumerable<GameObject> ChildObjects => gameObject.GetChildren();
        
        [TitleGroup("Physics")]
        [OdinSerialize]
        protected float Speed { get; private set; }
        
        [OdinSerialize]
        protected float RotationSpeed { get; private set; }
        
        protected InputComponent InputComponent { get; set; }
        
        protected Rigidbody2D Rigidbody { get; private set; }
        
        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            InputComponent = GetComponent<InputComponent>();
        }

        void FixedUpdate()
        {
            Move();
            Rotate();
        }

        void Move()
        {
            var direction = TankBody.transform.right;
            var velocity = direction * Speed * InputComponent.InputMovement * Time.fixedDeltaTime;
            Rigidbody.velocity = velocity;
        }

        void Rotate()
        {
            var zRotation = TankBody.transform.rotation.eulerAngles.z - InputComponent.InputRotation * RotationSpeed * Time.fixedDeltaTime;
            var rotation = Quaternion.Euler(0, 0, zRotation);
            TankBody.transform.rotation = rotation;
        }
    }
}
