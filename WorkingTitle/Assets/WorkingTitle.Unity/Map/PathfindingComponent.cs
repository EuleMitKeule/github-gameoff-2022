using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Gameplay;
using WorkingTitle.Unity.Physics;

namespace WorkingTitle.Unity.Map
{
    [RequireComponent(typeof(MapComponent))]
    public class PathfindingComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        public FlowField FlowField { get; private set; }
        
        public Vector2Int TargetCellPosition { get; private set; }
        public Vector2Int TargetPositiveCellPosition { get; private set; }
        
        MapComponent MapComponent { get; set; }
        public EntityComponent PlayerEntityComponent { get; private set; }

        void Awake()
        {
            MapComponent = GetComponent<MapComponent>();
        }

        void Start()
        {
            PlayerEntityComponent = 
                GetComponentInChildren<PlayerComponent>()
                .GetComponent<EntityComponent>();
            
            PlayerEntityComponent.CellPositionChanged += OnPlayerCellPositionChanged;

            UpdateTarget();
            UpdateDirections();
        }
        
        public PathfindingCell GetCell(Vector2Int position)
        {
            if (FlowField is null)
            {
                Debug.LogWarning("Cannot get cell because FlowField doesn't exist.");
                return null;
            }

            if (FlowField.GridSize.x <= position.x ||
                  FlowField.GridSize.y <= position.y ||
                  position.x < 0 ||
                  position.y < 0)
            {
                
                return null;
            }
            
            return FlowField?.Cells[position.x][position.y];
        }

        void OnPlayerCellPositionChanged(object sender, Vector2Int position)
        {
            UpdateTarget();
            UpdateDirections();
        }

        void UpdateTarget()
        {
            if (!PlayerEntityComponent) return;
         
            TargetCellPosition = PlayerEntityComponent.CellPosition;
            TargetPositiveCellPosition = PlayerEntityComponent.PositiveCellPosition;
        }

        void UpdateDirections()
        {
            if (!PlayerEntityComponent) return;
            
            var obstaclePositions = MapComponent
                .ObstacleTilemaps
                .GetTilePositions()
                .ToPositive(MapComponent.Bounds)
                .ToList();

            FlowField = new FlowField(TargetPositiveCellPosition, obstaclePositions, MapComponent.GridSize);
            FlowField.CalcCosts();
            FlowField.CalcDirections();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!FlowField?.IsDirectionsCalculated ?? true) return;
            
            var bounds = MapComponent.Bounds;
            var walkablePositions = MapComponent
                .WalkableTilemaps
                .GetTilePositions()
                .ToList();
            
            foreach (var walkablePosition in walkablePositions)
            {
                var positivePosition = walkablePosition.ToPositive(bounds);
                var direction = FlowField.Cells[positivePosition.x][positivePosition.y].Direction;
                var worldPosition = MapComponent.ToWorld(walkablePosition);
                
                Gizmos.color = Color.red;
                Gizmos.DrawRay(worldPosition, direction.ToVector2() * 0.3f);
            }
        }
#endif
    }
}