using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        
        bool HasTargetPositionChanged { get; set; }
        
        Coroutine UpdateFlowFieldCoroutine { get; set; }
        
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

            var obstaclePositions = MapComponent
                .ObstacleTilemaps
                .GetTilePositions()
                .ToPositive(MapComponent.Bounds)
                .ToList();
            
            UpdateTarget();
            FlowField = CalcFlowField(obstaclePositions);
            
            UpdateFlowFieldCoroutine = StartCoroutine(UpdateFlowField());
        }

        IEnumerator UpdateFlowField()
        {
            while (true)
            {
                if (HasTargetPositionChanged)
                {
                    var obstaclePositions = MapComponent
                        .ObstacleTilemaps
                        .GetTilePositions()
                        .ToPositive(MapComponent.Bounds)
                        .ToList();
            
                    var thread = new Thread(() =>
                    {
                        UpdateTarget();
                        var flowField = CalcFlowField(obstaclePositions);
                        FlowField = flowField;
                    });

                    thread.Start();
                    
                    HasTargetPositionChanged = false;
                }
                
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        public PathfindingCell GetCell(Vector2Int position)
        {
            var positivePosition = position.ToPositive(MapComponent.Bounds);
            
            if (FlowField is null)
            {
                Debug.LogWarning("Cannot get cell because FlowField doesn't exist.");
                return null;
            }

            if (FlowField.GridSize.x <= positivePosition.x ||
                FlowField.GridSize.y <= positivePosition.y ||
                positivePosition.x < 0 ||
                positivePosition.y < 0)
            {
                
                return null;
            }
            
            return FlowField.Cells[positivePosition.x][positivePosition.y];
        }

        void OnPlayerCellPositionChanged(object sender, Vector2Int position)
        {
            HasTargetPositionChanged = true;
        }

        void UpdateTarget()
        {
            if (!PlayerEntityComponent) return;
         
            TargetCellPosition = PlayerEntityComponent.CellPosition;
            TargetPositiveCellPosition = PlayerEntityComponent.PositiveCellPosition;
        }

        FlowField CalcFlowField(List<Vector2Int> obstaclePositions)
        {
            if (!PlayerEntityComponent) return null;
            
            var flowField = new FlowField(TargetPositiveCellPosition, obstaclePositions, MapComponent.GridSize);
            flowField.CalcCosts();
            flowField.CalcDirections();

            return flowField;
        }
    }
}