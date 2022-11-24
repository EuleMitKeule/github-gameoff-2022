using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets;
using TanksOnAPlain.Unity.Components.Map.Pathfinding;
using TanksOnAPlain.Unity.Components.Physics;
using TanksOnAPlain.Unity.Components.Pooling;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Ai
{
    public class AiComponent : StateContextComponent<AiStateComponent>, IDestroyable
    {
        [OdinSerialize]
        AiAsset AiAsset { get; set; }
        
        [TitleGroup("Target")]
        [ShowInInspector]
        [ReadOnly]
        PathfindingCell CurrentCell { get; set; }
            
        [ShowInInspector]
        [ReadOnly]
        Vector2 PathDirection { get; set; }

        [ShowInInspector]
        [ReadOnly]
        public Vector2 TargetDirection { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        float TargetDistance { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public bool IsPlayerVisible { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public bool IsWithinAimThreshold { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float PathAngle { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float AimAngle { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public bool IsWithinTargetDirectionThreshold { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public bool IsWithinTargetDistanceThreshold { get; set; }

        PathfindingComponent PathfindingComponent { get; set; }
        EnemyTankComponent TankComponent { get; set; }
        EntityComponent EntityComponent { get; set; }
        EntityComponent TargetEntityComponent { get; set; }

        public event EventHandler Destroyed;

        protected override void Awake()
        {
            base.Awake();
            
            TankComponent = GetComponent<EnemyTankComponent>();
            EntityComponent = GetComponent<EntityComponent>();
            PathfindingComponent = FindObjectOfType<PathfindingComponent>();
            TargetEntityComponent = FindObjectOfType<GameComponent>()
                .PlayerObject
                .GetComponent<EntityComponent>();

            EntityComponent.CellPositionChanged += OnCellPositionChanged;
        }

        protected override void Update()
        {
            base.Update();
            
            UpdateTarget();
        }

        void UpdateTarget()
        {
            if (!PathfindingComponent) return;

            var bodyPosition = (Vector2)TankComponent.TankBody.transform.position;
            var bodyUp = (Vector2)TankComponent.TankBody.transform.up;
            var cannonUp = (Vector2)TankComponent.TankCannon.transform.up;
            
            TargetDistance = (TargetEntityComponent.Position - bodyPosition).magnitude;
            TargetDirection = (TargetEntityComponent.Position - bodyPosition).normalized;
            IsPlayerVisible = !Physics2D.Raycast(
                bodyPosition,
                TargetDirection,
                TargetDistance,
                TankComponent.TankAsset.WallMask);
            AimAngle = Vector2.SignedAngle(TargetDirection, cannonUp);
            IsWithinAimThreshold = Mathf.Abs(AimAngle) < AiAsset.AimOnTargetThreshold;
            PathAngle = Vector2.SignedAngle(PathDirection, bodyUp);
            IsWithinTargetDirectionThreshold = Mathf.Abs(PathAngle) < AiAsset.TargetDirectionThreshold;
            IsWithinTargetDistanceThreshold = TargetDistance < AiAsset.TargetDistanceThreshold;
        }
        
        void OnCellPositionChanged(object sender, CellPositionChangedEventArgs e)
        {
            if (!PathfindingComponent) return;
            
            CurrentCell = PathfindingComponent.GetCell(e.NewCellPosition);

            if (CurrentCell is null || CurrentCell.IsObstacle)
            {
                Destroyed?.Invoke(this, EventArgs.Empty);
                return;
            }

            PathDirection = CurrentCell.Direction;
        }
    }
}