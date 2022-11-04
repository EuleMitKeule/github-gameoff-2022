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
        [TitleGroup("AI")]
        [OdinSerialize]
        float TargetDirectionThreshold { get; set; }

        [OdinSerialize]
        float TargetDistanceThreshold { get; set; }
        
        [TitleGroup("Target")]
        [ShowInInspector]
        [ReadOnly]
        PathfindingCell CurrentCell { get; set; }
            
        [ShowInInspector]
        [ReadOnly]
        Vector2 PathDirection { get; set; }

        [ShowInInspector]
        [ReadOnly]
        Vector2 TargetDirection { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        float TargetDistance { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        bool IsPlayerVisible { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        float PathAngle { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        bool WithinTargetDirectionThreshold { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        bool WithinTargetDistanceThreshold { get; set; }

        TankComponent TankComponent { get; set; }
        PathfindingComponent PathfindingComponent { get; set; }
        EntityComponent EntityComponent { get; set; }
        
        void Awake()
        {
            PathfindingComponent = GetComponentInParent<PathfindingComponent>();
            EntityComponent = GetComponent<EntityComponent>();
            TankComponent = GetComponent<TankComponent>();
            
            EntityComponent.CellPositionChanged += OnCellPositionChanged;
        }

        void OnCellPositionChanged(object sender, Vector2Int position)
        {
            if (!PathfindingComponent) return;
            
            CurrentCell = PathfindingComponent.GetCell(EntityComponent.PositiveCellPosition);

            if (CurrentCell is null) return;

            PathDirection = CurrentCell.Direction.ToVector2();
        }

        void Update()
        {
            UpdateTarget();
            
            InputRotation = WithinTargetDirectionThreshold ? 0 : Mathf.Sign(PathAngle);
            InputMovement = WithinTargetDistanceThreshold && IsPlayerVisible ? 0 : 1;
            InputAimPosition = PathfindingComponent.TargetPosition;
        }

        void UpdateTarget()
        {
            if (!PathfindingComponent) return;

            var bodyPosition = (Vector2)TankComponent.TankBody.transform.position;
            var bodyUp = (Vector2)TankComponent.TankBody.transform.up;
            
            TargetDistance = (PathfindingComponent.TargetPosition - bodyPosition).magnitude;
            TargetDirection = (PathfindingComponent.TargetPosition - bodyPosition).normalized;
            IsPlayerVisible = !Physics2D.Raycast(
                bodyPosition,
                TargetDirection,
                TargetDistance,
                TankComponent.WallMask);
            PathAngle = Vector2.SignedAngle(PathDirection, bodyUp);
            WithinTargetDirectionThreshold = Mathf.Abs(PathAngle) < TargetDirectionThreshold;
            WithinTargetDistanceThreshold = TargetDistance < TargetDistanceThreshold;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!TankComponent) return;
            
            var position = TankComponent.TankBody.transform.position;
            
            Gizmos.color = WithinTargetDirectionThreshold ? Color.green : Color.red;
            Gizmos.DrawLine(position, position + TankComponent.TankBody.transform.up);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(position, position + (Vector3)PathDirection);
        }
#endif
    }
}