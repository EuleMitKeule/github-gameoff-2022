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

        public Vector2 TargetPosition { get; private set; }
        
        MapComponent MapComponent { get; set; }
        CellEntityComponent PlayerCellEntityComponent { get; set; }
        
        void Awake()
        {
            MapComponent = GetComponent<MapComponent>();
            PlayerCellEntityComponent = 
                GetComponentInChildren<PlayerComponent>()?
                .GetComponent<CellEntityComponent>();

            if (PlayerCellEntityComponent)
            {
                PlayerCellEntityComponent.CellPositionChanged += OnPlayerCellPositionChanged;
            }

            UpdateTarget();
            UpdateDirections();
        }
        
        public PathfindingCell GetCell(Vector2Int position) => 
            FlowField?.Cells[position.x][position.y];

        void OnPlayerCellPositionChanged(object sender, Vector2Int position)
        {
            UpdateTarget();
            UpdateDirections();
        }

        void UpdateTarget()
        {
            if (!PlayerCellEntityComponent) return;
         
            TargetCellPosition = PlayerCellEntityComponent.CellPosition;
            TargetPositiveCellPosition = PlayerCellEntityComponent.PositiveCellPosition;
            TargetPosition = PlayerCellEntityComponent.Position;
        }

        void UpdateDirections()
        {
            if (!PlayerCellEntityComponent) return;
            
            var obstaclePositions = MapComponent
                .ObstacleTilemaps
                .GetTilePositions()
                .ToPositive(MapComponent.Bounds)
                .ToList();

            try
            {
                FlowField = new FlowField(TargetPositiveCellPosition, obstaclePositions, MapComponent.GridSize);
                FlowField.CalcCosts();
                FlowField.CalcDirections();
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!FlowField?.IsDirectionsCalculated ?? true) return;
            
            var bounds = MapComponent
                .Tilemaps
                .GetBounds();
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