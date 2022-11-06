using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Map;

namespace WorkingTitle.Unity.Physics
{
    public class EntityComponent : SerializedMonoBehaviour
    {
        [TitleGroup("Position")]
        [ShowInInspector]
        [ReadOnly]
        public Vector2Int CellPosition { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public Vector2Int PositiveCellPosition { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public Vector2 Position => transform.position;
        
        [ShowInInspector]
        [ReadOnly]
        public Direction ChunkDirection { get; private set; }
        
        public event EventHandler<Vector2Int> CellPositionChanged;
        public event EventHandler<Direction> ChunkChanged;

        MapComponent MapComponent { get; set; }

        void Start()
        {
            MapComponent = GetComponentInParent<MapComponent>();
            
            UpdateCellPosition(true);
            UpdateChunkDirection();
        }
        
        void Update()
        {
            UpdateCellPosition();
            UpdateChunkDirection();
        }

        void UpdateCellPosition(bool force = false)
        {
            if (!MapComponent) return;
            
            var currentCellPosition = MapComponent.ToCell(Position);

            if (currentCellPosition == CellPosition && !force) return;
            
            CellPosition = currentCellPosition;
            PositiveCellPosition = CellPosition.ToPositive(MapComponent.Bounds);
            CellPositionChanged?.Invoke(this, CellPosition);
        }

        void UpdateChunkDirection()
        {
            if (!MapComponent) return;
            
            var currentChunkDirection = MapComponent.ToChunkDirection(CellPosition);
            
            if (currentChunkDirection == ChunkDirection) return;
            ChunkDirection = currentChunkDirection;
            ChunkChanged?.Invoke(this, ChunkDirection);
        }
    }
}