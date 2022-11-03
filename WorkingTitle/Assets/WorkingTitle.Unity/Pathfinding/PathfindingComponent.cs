using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Input;

namespace WorkingTitle.Unity.Pathfinding
{
    public class PathfindingComponent : SerializedMonoBehaviour
    {
        IEnumerable<Grid> GridValues => GetComponentsInChildren<Grid>();
        IEnumerable<Tilemap> TilemapValues => GetComponentsInChildren<Tilemap>();
        IEnumerable<Transform> PlayerTransformValues => 
            FindObjectsOfType<Transform>().Where(e => e.GetComponent<PlayerInputComponent>());
        
        bool IsTilemapsUnique => !WalkableTilemaps.Any(e => ObstacleTilemaps.Contains(e));
        bool IsWalkableTilemapsNotEmpty => WalkableTilemaps.Any();
        
        [OdinSerialize]
        [Required]
        [ValueDropdown(nameof(GridValues))]
        Grid Grid { get; set; }
        
        [TitleGroup("Grid")]
        [OdinSerialize]
        [Required]
        [ValueDropdown(nameof(TilemapValues), IsUniqueList = true, ExcludeExistingValuesInList = true, DrawDropdownForListElements = false)]
        [ValidateInput(nameof(IsTilemapsUnique), "Tilemap can not be both walkable and obstacle.")]
        [ValidateInput(nameof(IsWalkableTilemapsNotEmpty), "At least one walkable tilemap is required.")]
        List<Tilemap> WalkableTilemaps { get; set; }
        
        [OdinSerialize]
        [Required]
        [ValueDropdown(nameof(TilemapValues), IsUniqueList = true, ExcludeExistingValuesInList = true, DrawDropdownForListElements = false)]
        [ValidateInput(nameof(IsTilemapsUnique), "Tilemap can not be both walkable and obstacle.")]
        List<Tilemap> ObstacleTilemaps { get; set; }
        
        List<Tilemap> Tilemaps => WalkableTilemaps.Concat(ObstacleTilemaps).ToList();
        
        public PathfindingDirection[,] Directions { get; set; }
        
        [OdinSerialize]
        [Required]
        [ValueDropdown(nameof(PlayerTransformValues))]
        Transform PlayerTransform { get; set; }
        
        Vector2Int? LastPlayerPosition { get; set; }

        void Update()
        {
            UpdatePlayerPosition();
        }

        void UpdatePlayerPosition()
        {
            var playerPosition = (Vector2Int)Grid.WorldToCell(PlayerTransform.position);
            if (LastPlayerPosition != null && playerPosition == LastPlayerPosition.Value) return;
            
            LastPlayerPosition = playerPosition;
            UpdateDirections();
        }

        void UpdateDirections()
        {
            if (!LastPlayerPosition.HasValue) return;
            
            var bounds = Tilemaps.GetBounds();
            var targetPosition = LastPlayerPosition.Value.ToPositive(bounds);
            var gridSize = (Vector2Int)bounds.ToPositive().size;
            var obstaclePositions = ObstacleTilemaps
                .GetTilePositions()
                .ToPositive(bounds)
                .ToList();
            
            var flowField = new FlowField(targetPosition, obstaclePositions, gridSize);
            flowField.CalcCosts();
            flowField.CalcDirections();
            Directions = flowField.GetDirections();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (Directions == null) return;
            
            var bounds = Tilemaps.GetBounds();
            var walkablePositions = WalkableTilemaps
                .GetTilePositions()
                .ToList();
        
            foreach (var walkablePosition in walkablePositions)
            {
                var positivePosition = walkablePosition.ToPositive(bounds);
                var direction = Directions[positivePosition.x, positivePosition.y];
                var worldPosition = Grid.CellToWorld((Vector3Int)walkablePosition) + Grid.cellSize / 2;
                
                Gizmos.color = Color.red;
                Gizmos.DrawRay(worldPosition, direction.ToVector2() * 0.3f);
            }
        }
#endif
    }
}