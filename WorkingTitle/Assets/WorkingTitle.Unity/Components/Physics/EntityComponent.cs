using System;
using Sirenix.OdinInspector;
using UnityEngine;
using WorkingTitle.Lib.Extensions;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Components.Map;
using WorkingTitle.Unity.Extensions;

namespace WorkingTitle.Unity.Components.Physics
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
        
        public event EventHandler<CellPositionChangedEventArgs> CellPositionChanged;
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
        }

        void UpdateCellPosition(bool ignoreChange = false)
        {
            if (!MapComponent) return;
            
            var currentCellPosition = Position.ToCell();

            if (currentCellPosition == CellPosition) return;

            var oldCellPosition = CellPosition;
            CellPosition = currentCellPosition;
            
            UpdateChunkDirection();
            
            CellPositionChanged?.Invoke(this, new CellPositionChangedEventArgs(CellPosition, ignoreChange ? CellPosition : oldCellPosition));
        }

        void UpdateChunkDirection()
        {
            if (!MapComponent) return;

            var relativeCellPosition = CellPosition - MapComponent.ChunkPosition;
            
            var horizontalDirection = relativeCellPosition.x < 0
                ? Direction.Left
                : relativeCellPosition.x >= MapComponent.MapAsset.ChunkSize ? 
                    Direction.Right :
                    Direction.None;
            var verticalDirection = relativeCellPosition.y < 0
                ? Direction.Down
                : relativeCellPosition.y >= MapComponent.MapAsset.ChunkSize ? 
                    Direction.Up :
                    Direction.None;
            
            var direction = (horizontalDirection.ToVector2Int() + verticalDirection.ToVector2Int()).ToDirection();
            
            if (direction == ChunkDirection) return;
            
            ChunkDirection = direction;
            ChunkChanged?.Invoke(this, ChunkDirection);
        }
    }
}