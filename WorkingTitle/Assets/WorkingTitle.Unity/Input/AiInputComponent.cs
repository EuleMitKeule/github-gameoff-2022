using System;
using System.Numerics;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Gameplay;
using WorkingTitle.Unity.Map;
using WorkingTitle.Unity.Physics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace WorkingTitle.Unity.Input
{
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(TankComponent))]
    public class AiInputComponent : InputComponent
    {
        [OdinSerialize]
        float AimRotationThreshold { get; set; }

        public override AimMode SelectedAimMode => AimMode.Rotational;
        
        TankComponent TankComponent { get; set; }

        void Awake()
        {
            TankComponent = GetComponent<TankComponent>();
        }

        void Update()
        {
            var currentAimDirection = TankComponent.TankCannon.transform.up;
            var angle = Vector2.SignedAngle(currentAimDirection, InputAimDirection);
            var rotationSign = (int)Mathf.Sign(angle);
            InputAimRotation = Mathf.Abs(angle) > AimRotationThreshold ? rotationSign : 0;
        }
        
        public override void EnableInput()
        {
            throw new System.NotImplementedException();
        }

        public override void DisableInput()
        {
            throw new System.NotImplementedException();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!TankComponent) return;
            if (!TankComponent.TankCannon) return;
            var currentAimDirection = TankComponent.TankCannon.transform.up;
            Debug.DrawRay(transform.position, currentAimDirection, Color.red);
            Debug.DrawRay(transform.position, InputAimDirection, Color.green);
        }
#endif
    }
}