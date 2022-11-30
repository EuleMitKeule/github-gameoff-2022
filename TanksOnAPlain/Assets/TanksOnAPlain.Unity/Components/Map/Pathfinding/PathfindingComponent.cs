using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Sirenix.OdinInspector;
using TanksOnAPlain.Unity.Components.Physics;
using TanksOnAPlain.Unity.Extensions;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Map.Pathfinding
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
                FindObjectOfType<GameComponent>()
                    .PlayerObject
                    .GetComponent<EntityComponent>();
            
            PlayerEntityComponent.CellPositionChanged += OnPlayerCellPositionChanged;

            var obstaclePositions = MapComponent
                .GetObstacleTilemaps()
                .GetTilePositions(MapComponent.MapBounds)
                .ToList();
            
            UpdateTarget();
            FlowField = CalcFlowField(obstaclePositions);
            
            UpdateFlowFieldCoroutine = StartCoroutine(UpdateFlowField());
        }

        IEnumerator UpdateFlowField()
        {
            while (gameObject.activeSelf)
            {
                if (HasTargetPositionChanged)
                {
                    var obstaclePositions = MapComponent
                        .GetObstacleTilemaps()
                        .GetTilePositions(MapComponent.MapBounds)
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
            if (FlowField is null)
            {
                Debug.LogWarning("Cannot get cell because FlowField doesn't exist.");
                return null;
            }

            if (position.x < FlowField.MapBounds.xMin || position.x >= FlowField.MapBounds.xMax ||
                position.y < FlowField.MapBounds.yMin || position.y >= FlowField.MapBounds.yMax)
                return null;
            
            return FlowField.Cells[position];
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
            
            var flowField = new FlowField(
                TargetCellPosition, 
                obstaclePositions, 
                MapComponent.MapBounds);
            flowField.CalcCosts();
            flowField.CalcDirections();

            return flowField;
        }
            
// #if UNITY_EDITOR
//     
//         void OnDrawGizmos()
//         {
//             foreach (var (position, cell) in FlowField.Cells)
//             {
//                 var worldPosition = position.ToWorld();
//                     
//                 Debug.DrawRay(worldPosition, cell.Direction * 0.5f, Color.red);
//             }
//         }
//     
// #endif
    }
}