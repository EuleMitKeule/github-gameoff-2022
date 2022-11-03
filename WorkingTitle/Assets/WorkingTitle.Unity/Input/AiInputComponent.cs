using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Gameplay;
using WorkingTitle.Unity.Pathfinding;

namespace WorkingTitle.Unity.Input
{
    public class AiInputComponent : InputComponent
    {
        IEnumerable<Grid> GridValues => FindObjectsOfType<Grid>();
        
        [OdinSerialize]
        [Required]
        [ValueDropdown(nameof(GridValues))]
        Grid Grid { get; set; }
        TankComponent TankComponent { get; set; }
        PathfindingComponent PathfindingComponent { get; set; }

        Vector2Int GridPosition => ((Vector2Int) Grid.WorldToCell(transform.position)).ToPositive(PathfindingComponent.Bounds);
        Vector2 PathDirection =>
            PathfindingComponent ?
            PathfindingComponent.FlowField?.Cells[GridPosition.x][GridPosition.y]?.Direction.ToVector2() ?? Vector2.zero :
            Vector2.zero;

        [ShowInInspector]
        Vector2 TargetDirection => (PathfindingComponent.PlayerTransform.position - TankComponent.TankBody.transform.position).normalized;
        [OdinSerialize]
        float TargetDirectionThreshold { get; set; }
        [ShowInInspector]
        bool WithinTargetDirectionThreshold => Mathf.Abs(PathAngle) < TargetDirectionThreshold;
        
        [OdinSerialize]
        float TargetDistanceThreshold { get; set; }
        [ShowInInspector]
        float TargetDistance => (PathfindingComponent.PlayerTransform.position - TankComponent.TankBody.transform.position).magnitude;
        [ShowInInspector]
        bool WithinTargetDistanceThreshold => TargetDistance < TargetDistanceThreshold;
        [ShowInInspector]
        bool IsPlayerVisible => !Physics2D.Raycast(TankComponent.TankBody.transform.position, TargetDirection, TargetDistance,
            TankComponent.WallMask);
        
        [ShowInInspector]
        float PathAngle => TankComponent ? Vector2.SignedAngle(PathDirection, TankComponent.TankBody.transform.up) : 0;

        void Awake()
        {
            TankComponent = GetComponent<TankComponent>();
            PathfindingComponent = Grid.GetComponent<PathfindingComponent>();
        }

        void Update()
        {
            InputRotation = WithinTargetDirectionThreshold ? 0 : Mathf.Sign(PathAngle);
            InputMovement = WithinTargetDistanceThreshold && IsPlayerVisible ? 0 : 1;
            InputAimPosition = PathfindingComponent.PlayerTransform.position;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!TankComponent) return;
            
            Gizmos.color = WithinTargetDirectionThreshold ? Color.green : Color.red;
            Gizmos.DrawLine(TankComponent.TankBody.transform.position, TankComponent.TankBody.transform.position + TankComponent.TankBody.transform.up);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(TankComponent.TankBody.transform.position, TankComponent.TankBody.transform.position + (Vector3)PathDirection);
        }
#endif
    }
}