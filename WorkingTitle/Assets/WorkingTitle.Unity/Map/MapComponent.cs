using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorkingTitle.Unity.Extensions;

namespace WorkingTitle.Unity.Map
{
    [RequireComponent(typeof(Grid))]
    public class MapComponent : SerializedMonoBehaviour
    {
        [TitleGroup("Map")]
        [ShowInInspector]
        [ReadOnly]
        public List<Tilemap> Tilemaps { get; private set; } = new();
        
        [ShowInInspector]
        [ReadOnly]
        public List<Tilemap> WalkableTilemaps { get; private set; } = new();
        
        [ShowInInspector]
        [ReadOnly]
        public List<Tilemap> ObstacleTilemaps { get; private set; } = new();
        
        [ShowInInspector]
        [ReadOnly]
        public BoundsInt Bounds { get; private set; }
        
        [ShowInInspector]
        [ReadOnly]
        public Vector2Int GridSize { get; private set; }
        
        List<ChunkComponent> ChunkComponents { get; set; } = new();
        Grid Grid { get; set; }
        
        void Awake()
        {
            Grid = GetComponent<Grid>();

            UpdateChunks();
        }

        void UpdateChunks()
        {
            ChunkComponents = GetComponentsInChildren<ChunkComponent>().ToList();
            
            WalkableTilemaps = ChunkComponents
                .SelectMany(c => c.WalkableTilemaps)
                .ToList();
            ObstacleTilemaps = ChunkComponents
                .SelectMany(c => c.ObstacleTilemaps)
                .ToList();
            Tilemaps = ChunkComponents
                .SelectMany(c => c.Tilemaps)
                .ToList();
            
            Bounds = Tilemaps.GetBounds();
            GridSize = (Vector2Int)Bounds.ToPositive().size;
        }
        
        public Vector2 ToWorld(Vector2Int position) =>
            Grid.CellToWorld((Vector3Int)position) + Grid.cellSize / 2;

        public Vector2Int ToCell(Vector2 position) =>
            (Vector2Int)Grid.WorldToCell(position);
    }
}