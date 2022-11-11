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
        public int ChunkSize { get; set; }

        [OdinSerialize]
        [ValidateInput(nameof(IsAtLeastOneChunk), "There must be at least one chunk")]
        [ValidateInput(nameof(IsChunksSameSize), "All chunks must be the same size")]
        public List<GameObject> ChunkPrefabs { get; set; } = new();
        
        [OdinSerialize]
        Dictionary<TilemapType, Tilemap> Tilemaps { get; set; } = new();
        
        [OdinSerialize]
        Dictionary<TilemapType, bool> TilemapIsWalkable { get; set; } = new();

        [ShowInInspector]
        [ReadOnly]
        public Vector2Int MapSize { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public BoundsInt MapBounds { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public BoundsInt CenterChunkBounds { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        Vector2Int CenterChunkPosition { get; set; }
        
        Grid Grid { get; set; }
        EntityComponent PlayerEntityComponent { get; set; }

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
            MapSize = new Vector2Int(3 * ChunkSize, 3 * ChunkSize);
        }

        void Start()
        {
            PlayerEntityComponent = 
                GetComponentInChildren<PlayerComponent>()
                .GetComponent<EntityComponent>();
            
            if (PlayerEntityComponent)
            {
                PlayerEntityComponent.ChunkChanged += OnChunkChanged;
            }
            
            SpawnChunks(DirectionExtensions.All);
            UpdateBounds();
        }

        void SpawnChunk(Direction direction)
        {
            var randomChunkIndex = Random.Range(0, ChunkPrefabs.Count);
            var randomChunk = ChunkPrefabs[randomChunkIndex];
            var chunkComponent = randomChunk.GetComponent<ChunkComponent>();
            
            var chunkPosition = (Vector3Int)(CenterChunkPosition + direction.ToVector2Int() * ChunkSize);
            var fromBounds = new BoundsInt(Vector3Int.zero, new Vector3Int(ChunkSize, ChunkSize, 1));
            var toBounds = new BoundsInt(chunkPosition, new Vector3Int(ChunkSize, ChunkSize, 1));
            
            foreach (var (sourceTilemapType, sourceTilemap) in chunkComponent.Tilemaps)
            {
                if (!Tilemaps.Keys.Contains(sourceTilemapType)) continue;
                
                var destinationTilemap = Tilemaps[sourceTilemapType];
                
                var tiles = sourceTilemap.GetTilesBlock(fromBounds);
                destinationTilemap.SetTilesBlock(toBounds, tiles);
                
                destinationTilemap.CompressBounds();
            }
        }

        void SpawnChunks(IEnumerable<Direction> directions)
        {
            foreach (var direction in directions)
            {
                SpawnChunk(direction);
            }
        }

        void DeleteChunk(Direction direction)
        {
            var chunkPosition = (Vector3Int)(CenterChunkPosition + direction.ToVector2Int() * ChunkSize);
            var bounds = new BoundsInt(chunkPosition, new Vector3Int(ChunkSize, ChunkSize, 1));
            var tiles = new TileBase[ChunkSize * ChunkSize];
            
            foreach (var (_, tilemap) in Tilemaps)
            {
                tilemap.SetTilesBlock(bounds, tiles);
                tilemap.CompressBounds();
            }
        }
        
        void DeleteChunks(IEnumerable<Direction> directions)
        {
            foreach (var direction in directions)
            {
                DeleteChunk(direction);
            }
        }

        void OnChunkChanged(object sender, Direction direction)
        {
            if (direction == Direction.None) return;
            
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
            
            CenterChunkPosition += direction.ToVector2Int() * ChunkSize;
            
            SpawnChunks(directionsToAdd);
            UpdateBounds();
            
        }

        void UpdateBounds()
        {
            var mapBounds = new BoundsInt(
                (Vector3Int)CenterChunkPosition - new Vector3Int(ChunkSize, ChunkSize), 
                new Vector3Int(ChunkSize * 3, ChunkSize * 3, 1));
            mapBounds.ClampToBounds(mapBounds);
            MapBounds = mapBounds;
            
            var centerBounds = new BoundsInt((Vector3Int)CenterChunkPosition, new Vector3Int(ChunkSize, ChunkSize, 1));
            centerBounds.ClampToBounds(centerBounds);
            CenterChunkBounds = centerBounds;
        }
        
        IEnumerable<Tilemap> GetWalkableTilemaps() => TilemapIsWalkable
            .Keys
            .Where(e => TilemapIsWalkable[e])
            .Select(e => Tilemaps[e]);

        public IEnumerable<Tilemap> GetObstacleTilemaps() => TilemapIsWalkable
            .Keys
            .Where(e => !TilemapIsWalkable[e])
            .Select(e => Tilemaps[e]);
    }
}