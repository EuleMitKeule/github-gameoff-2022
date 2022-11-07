using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Map;
using WorkingTitle.Unity.Physics;

namespace WorkingTitle.Unity.Gameplay.Ai
{
    public class AiComponent : StateContextComponent<AiStateComponent>
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

        PathfindingComponent PathfindingComponent { get; set; }
        TankComponent TankComponent { get; set; }
        EntityComponent EntityComponent { get; set; }

        void Awake()
        {
            TankComponent = GetComponent<TankComponent>();
            EntityComponent = GetComponent<EntityComponent>();

            EntityComponent.CellPositionChanged += OnCellPositionChanged;
        }
        
        void Start()
        {
            PathfindingComponent = GetComponentInParent<PathfindingComponent>();
        }

        void Update()
        {
            UpdateTarget();
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
        
        void OnCellPositionChanged(object sender, Vector2Int position)
        {
            if (!PathfindingComponent) return;
            
            CurrentCell = PathfindingComponent.GetCell(EntityComponent.PositiveCellPosition);

            if (CurrentCell is null) return;

            PathDirection = CurrentCell.Direction.ToVector2();
        }
    }
}