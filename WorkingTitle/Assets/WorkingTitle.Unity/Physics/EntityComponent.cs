﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Lib.Extensions;
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
            ChunkDirection = Direction.None;
        }
        
        void Update()
        {
            UpdateCellPosition();
            UpdateChunkDirection();
        }

        void UpdateCellPosition(bool force = false)
        {
            if (!MapComponent) return;
            
            var currentCellPosition = Position.ToCell();

            if (currentCellPosition == CellPosition && !force) return;
            
            CellPosition = currentCellPosition;
            
            CellPositionChanged?.Invoke(this, CellPosition);
        }

        void UpdateChunkDirection()
        {
            if (!MapComponent) return;

            var centerBounds = MapComponent.CenterChunkBounds;

            var horizontalDirection = CellPosition.x < centerBounds.xMin
                ? Direction.Left
                : CellPosition.x > centerBounds.xMax ? 
                    Direction.Right :
                    Direction.None;
            var verticalDirection = CellPosition.y < centerBounds.yMin
                ? Direction.Down
                : CellPosition.y > centerBounds.yMax ? 
                    Direction.Up :
                    Direction.None;
            
            var direction = (horizontalDirection.ToVector2Int() + verticalDirection.ToVector2Int()).ToDirection();
            
            if (direction == ChunkDirection) return;
            
            ChunkDirection = direction;
            ChunkChanged?.Invoke(this, ChunkDirection);
        }
    }
}