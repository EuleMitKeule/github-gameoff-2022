using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Gameplay;
using WorkingTitle.Unity.Physics;
using Random = UnityEngine.Random;

namespace WorkingTitle.Unity.Map
{
    [RequireComponent(typeof(Grid))]
    public class MapComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ValidateInput(nameof(IsAtLeastOneChunk), "There must be at least one chunk")]
        [ValidateInput(nameof(IsChunksSameSize), "All chunks must be the same size")]
        public List<GameObject> ChunkPrefabs { get; set; } = new();

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

        [ShowInInspector]
        [ReadOnly]
        public Dictionary<Direction, ChunkComponent> ChunkComponents { get; private set; } = new();
        
        [ShowInInspector]
        [ReadOnly]
        Direction PlayerChunk { get; set; }
        
        public event EventHandler<Direction> ChunkChanged;

        Grid Grid { get; set; }
        CellEntityComponent PlayerCellEntityComponent { get; set; }

        #region Editor

        bool IsAtLeastOneChunk => ChunkPrefabs.Count > 0;

        bool IsChunksSameSize => ChunkPrefabs
            .Select(chunkPrefab => chunkPrefab.GetComponentsInChildren<Tilemap>().GetBounds())
            .Distinct()
            .Count() == 1;
        
        #endregion
        
        void Awake()
        {
            Grid = GetComponent<Grid>();
            PlayerCellEntityComponent =
                GetComponentInChildren<PlayerComponent>()?
                    .GetComponent<CellEntityComponent>();

            if (PlayerCellEntityComponent)
            {
                PlayerCellEntityComponent.CellPositionChanged += OnPlayerCellPositionChanged;
            }
            
            SpawnInitialChunk();
            SpawnChunks(DirectionExtensions.CardinalAndInterCardinal);

            UpdateChunks();
            UpdatePlayerChunk();
            
            ChunkChanged += OnChunkChanged;
        }

        void OnPlayerCellPositionChanged(object sender, Vector2Int position)
        {
            UpdatePlayerChunk();
        }

        void OnChunkChanged(object sender, Direction direction)
        {
            var directionsToAdd = direction switch
            {
                _ when DirectionExtensions.Upwards.Contains(direction) => DirectionExtensions.Upwards.ToList(),
                _ when DirectionExtensions.Leftwards.Contains(direction) => DirectionExtensions.Leftwards.ToList(),
                _ when DirectionExtensions.Downwards.Contains(direction) => DirectionExtensions.Downwards.ToList(),
                _ when DirectionExtensions.Rightwards.Contains(direction) => DirectionExtensions.Rightwards.ToList(),
                _ => new List<Direction>()
            };

            var directionsToDelete = directionsToAdd.ToOpposite();

            DeleteChunks(directionsToDelete);
            MoveChunks(direction.ToOpposite());
            SpawnChunks(directionsToAdd);
            UpdateChunks();
        }
        
        void DeleteChunks(IEnumerable<Direction> directions)
        {
            foreach (var direction in directions)
            {
                if (!ChunkComponents[direction]) continue;
                
                var chunk = ChunkComponents[direction].gameObject;
                Destroy(chunk);
                ChunkComponents[direction] = null;
            }
        }

        void MoveChunks(Direction direction)
        {
            var chunkComponents = new Dictionary<Direction, ChunkComponent>();
            var directionsNotToMove = direction switch
            {
                _ when DirectionExtensions.Upwards.Contains(direction) => DirectionExtensions.Upwards.ToList(),
                _ when DirectionExtensions.Leftwards.Contains(direction) => DirectionExtensions.Leftwards.ToList(),
                _ when DirectionExtensions.Downwards.Contains(direction) => DirectionExtensions.Downwards.ToList(),
                _ when DirectionExtensions.Rightwards.Contains(direction) => DirectionExtensions.Rightwards.ToList(),
                _ => new List<Direction>()
            };
            
            var directionsToMove = DirectionExtensions.All.Except(directionsNotToMove);
            
            foreach (var chunkDirection in directionsToMove)
            {
                var chunkComponent = ChunkComponents[chunkDirection];
                
                if (!chunkComponent) continue;
                
                var newDirection = chunkDirection.MoveDirection(direction);
                
                chunkComponents[newDirection] = chunkComponent;
            }

            ChunkComponents = chunkComponents;
        }
        
        void SpawnInitialChunk()
        {
            var randomIndex = Random.Range(0, ChunkPrefabs.Count);
            var chunkPrefab = ChunkPrefabs[randomIndex];
            
            var chunk = Instantiate(chunkPrefab, transform);
            var chunkComponent = chunk.GetComponent<ChunkComponent>();
            
            ChunkComponents[Direction.None] = chunkComponent;
        }

        void SpawnChunks(IEnumerable<Direction> directions)
        {
            foreach (var direction in directions)
            {
                SpawnChunk(direction);
            }
        }

        void SpawnChunk(Direction direction)
        {
            if (direction == Direction.None) return;
            if (!ChunkComponents[Direction.None]) return;
            
            var randomIndex = Random.Range(0, ChunkPrefabs.Count);
            var chunkPrefab = ChunkPrefabs[randomIndex];

            var centerBounds = ChunkComponents[Direction.None].Bounds;
            var newChunkBounds = new BoundsInt
            {
                xMin = direction switch
                {
                    Direction.Left or Direction.UpLeft or Direction.DownLeft => centerBounds.xMin -
                        centerBounds.size.x,
                    Direction.Right or Direction.UpRight or Direction.DownRight => centerBounds.xMax,
                    _ => centerBounds.xMin
                },
                xMax = direction switch
                {
                    Direction.Left or Direction.UpLeft or Direction.DownLeft => centerBounds.xMin,
                    Direction.Right or Direction.UpRight or Direction.DownRight => centerBounds.xMax +
                        centerBounds.size.x,
                    _ => centerBounds.xMax
                },
                yMin = direction switch
                {
                    Direction.Up or Direction.UpLeft or Direction.UpRight => centerBounds.yMax,
                    Direction.Down or Direction.DownLeft or Direction.DownRight => centerBounds.yMin -
                        centerBounds.size.y,
                    _ => centerBounds.yMin
                },
                yMax = direction switch
                {
                    Direction.Up or Direction.UpLeft or Direction.UpRight =>
                        centerBounds.yMax + centerBounds.size.y,
                    Direction.Down or Direction.DownLeft or Direction.DownRight => centerBounds.yMin,
                    _ => centerBounds.yMax
                },
                zMin = 0,
                zMax = 1
            };
            
            var chunk = Instantiate(chunkPrefab, transform);
            var tilemaps = chunk.GetComponentsInChildren<Tilemap>();
            var oldChunkBounds = tilemaps.GetBounds();
            
            foreach (var tilemap in tilemaps)
            {
                tilemap.MoveTiles(oldChunkBounds, newChunkBounds);
            }
            
            var chunkComponent = chunk.GetComponent<ChunkComponent>();
            chunkComponent.Initialize();
            ChunkComponents[direction] = chunkComponent;
        }
        
        void UpdatePlayerChunk()
        {
            var playerChunk = ChunkComponents.GetChunk(ToCell(PlayerCellEntityComponent.Position));

            if (playerChunk == PlayerChunk) return;
            
            PlayerChunk = playerChunk;
            ChunkChanged?.Invoke(this, PlayerChunk);
        }

        void UpdateChunks()
        {
            var chunkComponents = ChunkComponents.Values;
            
            WalkableTilemaps = chunkComponents
                .SelectMany(c => c.WalkableTilemaps)
                .ToList();
            ObstacleTilemaps = chunkComponents
                .SelectMany(c => c.ObstacleTilemaps)
                .ToList();
            Tilemaps = chunkComponents
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