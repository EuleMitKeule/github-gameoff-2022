using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Gameplay;
using WorkingTitle.Unity.Map;
using WorkingTitle.Unity.Physics;

namespace WorkingTitle.Unity.Input
{
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(TankComponent))]
    public class AiInputComponent : InputComponent
    {
       

        void Update()
        {
            // InputRotation = WithinTargetDirectionThreshold ? 0 : Mathf.Sign(PathAngle);
            // InputMovement = WithinTargetDistanceThreshold && IsPlayerVisible ? 0 : 1;
            // InputAimPosition = PathfindingComponent.PlayerEntityComponent.transform.position;
            // InputPrimaryAttack = IsPlayerVisible;
        }
        
        public override void EnableInput()
        {
            throw new System.NotImplementedException();
        }

        public override void DisableInput()
        {
            throw new System.NotImplementedException();
        }
//
// #if UNITY_EDITOR
//         void OnDrawGizmos()
//         {
//             if (!TankComponent) return;
//             
//             var position = TankComponent.TankBody.transform.position;
//             
//             Gizmos.color = WithinTargetDirectionThreshold ? Color.green : Color.red;
//             Gizmos.DrawLine(position, position + TankComponent.TankBody.transform.up);
//             
//             Gizmos.color = Color.magenta;
//             Gizmos.DrawLine(position, position + (Vector3)PathDirection);
//         }
// #endif
    }
}