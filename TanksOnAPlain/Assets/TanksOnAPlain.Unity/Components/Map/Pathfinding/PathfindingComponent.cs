using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TanksOnAPlain.Unity.Components.Physics;
using TanksOnAPlain.Unity.Extensions;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace TanksOnAPlain.Unity.Components.Map.Pathfinding
{
    [RequireComponent(typeof(MapComponent))]
    public class PathfindingComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [Sirenix.OdinInspector.ReadOnly]
        public Dictionary<Vector2Int, PathfindingCell> Cells { get; private set; } = new();
        
        public Vector2Int TargetCellPosition { get; private set; }
        
        MapComponent MapComponent { get; set; }
        public EntityComponent PlayerEntityComponent { get; private set; }

        // NativeArray<Vector2Int> nativeObstaclePositions;
        // NativeHashMap<Vector2Int, PathfindingCell> nativeCells;

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
            
            UpdateTarget();
            StartCoroutine(UpdateCells());
        }

        public PathfindingCell? GetCell(Vector2Int position)
        {
            if (position.x < MapComponent.MapBounds.xMin || position.x > MapComponent.MapBounds.xMax ||
                position.y < MapComponent.MapBounds.yMin || position.y > MapComponent.MapBounds.yMax)
                return null;

            if (!Cells.ContainsKey(position)) return null;
            
            return Cells[position];
        }

        void OnPlayerCellPositionChanged(object sender, CellPositionChangedEventArgs e)
        {
            UpdateTarget();
            StartCoroutine(UpdateCells());
        }

        void UpdateTarget()
        {
            if (!PlayerEntityComponent) return;
         
            TargetCellPosition = PlayerEntityComponent.CellPosition;
        }

        IEnumerator UpdateCells()
        {
            if (!PlayerEntityComponent) yield break;

            var nativeCells = new NativeHashMap<Vector2Int, PathfindingCell>(
                (MapComponent.MapBounds.size.x + 1) * (MapComponent.MapBounds.size.y + 1), 
                Allocator.Persistent);
            
            var obstaclePositions = MapComponent
                .GetObstacleTilemaps()
                .GetTilePositions(MapComponent.MapBounds)
                .ToArray();

            var nativeObstaclePositions = new NativeArray<Vector2Int>(obstaclePositions, Allocator.Persistent);
            
            var job = new FlowFieldJob(
                MapComponent.MapBounds,
                TargetCellPosition,
                nativeObstaclePositions,
                nativeCells);

            var jobHandle = job.Schedule();
            
            while (!jobHandle.IsCompleted)
            {
                yield return null;
            }
            
            jobHandle.Complete();

            var cells = new Dictionary<Vector2Int, PathfindingCell>();
            foreach (var pair in job.Cells)
            {
                cells[pair.Key] = pair.Value;
            }

            Cells = cells;
            
            nativeObstaclePositions.Dispose();
            nativeCells.Dispose();
        }
        
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            foreach (var (position, cell) in Cells)
            {
                var worldPosition = position.ToWorld();
                Debug.DrawRay(worldPosition, cell.Direction * 0.5f, Color.red);
            }
        }
    
#endif
    }
}