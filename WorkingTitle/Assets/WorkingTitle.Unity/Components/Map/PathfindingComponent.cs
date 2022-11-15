using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Components.Physics;
using WorkingTitle.Unity.Extensions;

namespace WorkingTitle.Unity.Components.Map
{
    [RequireComponent(typeof(MapComponent))]
    public class PathfindingComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        public FlowField FlowField { get; private set; }
        
        public Vector2Int TargetCellPosition { get; private set; }
        
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
                .GetObstacleTilemaps()
                .GetTilePositions(MapComponent.MapBounds)
                .ToPositive(MapComponent.MapBounds)
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
                        .GetObstacleTilemaps()
                        .GetTilePositions(MapComponent.MapBounds)
                        .ToPositive(MapComponent.MapBounds)
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
            var positivePosition = position.ToPositive(MapComponent.MapBounds);
            
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

        void OnPlayerCellPositionChanged(object sender, CellPositionChangedEventArgs e)
        {
            HasTargetPositionChanged = true;
        }

        void UpdateTarget()
        {
            if (!PlayerEntityComponent) return;
         
            TargetCellPosition = PlayerEntityComponent.CellPosition;
        }

        FlowField CalcFlowField(List<Vector2Int> obstaclePositions)
        {
            if (!PlayerEntityComponent) return null;

            var targetPositiveCellPosition = TargetCellPosition.ToPositive(MapComponent.MapBounds);
            
            var flowField = new FlowField(
                targetPositiveCellPosition, 
                obstaclePositions, 
                MapComponent.MapSize);
            flowField.CalcCosts();
            flowField.CalcDirections();

            return flowField;
        }
            
#if UNITY_EDITOR
    
        void OnDrawGizmos()
        {
            foreach (var cells in FlowField.Cells)
            {
                foreach (var cell in cells)
                {
                    Debug.DrawRay((Vector3)(Vector2)cell.Position + MapComponent.MapBounds.position, cell.Direction * 0.5f, Color.red);
                }
            }
        }
    
#endif
    }
}