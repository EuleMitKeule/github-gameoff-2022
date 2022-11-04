using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Map;

namespace WorkingTitle.Unity.Physics
{
    public class CellEntityComponent : SerializedMonoBehaviour
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
        
        public event EventHandler<Vector2Int> CellPositionChanged;

        MapComponent MapComponent { get; set; }
        
        void Awake()
        {
            MapComponent = GetComponentInParent<MapComponent>();
            
            UpdateCellPosition();
        }
        
        void Update()
        {
            UpdateCellPosition();
        }

        void UpdateCellPosition()
        {
            if (!MapComponent) return;
            
            var currentCellPosition = MapComponent.ToCell(Position);

            if (currentCellPosition == CellPosition) return;
            
            CellPosition = currentCellPosition;
            PositiveCellPosition = CellPosition.ToPositive(MapComponent.Bounds);
            CellPositionChanged?.Invoke(this, CellPosition);
        }
    }
}