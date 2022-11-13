using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.Physics;
using Random = UnityEngine.Random;

namespace WorkingTitle.Unity.Components.Map
{
    [RequireComponent(typeof(Grid))]
    public class MapComponent : SerializedMonoBehaviour
    {
        [OdinSerialize] 
        public MapAsset MapAsset { get; set; }
        
        [OdinSerialize]
        public Dictionary<TilemapType, Tilemap> Tilemaps { get; set; } = new();

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

        void Awake()
        {
            Grid = GetComponent<Grid>();
            MapSize = new Vector2Int(3 * MapAsset.ChunkSize, 3 * MapAsset.ChunkSize);
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
            var randomChunkIndex = Random.Range(0, MapAsset.ChunkPrefabs.Count);
            var randomChunk = MapAsset.ChunkPrefabs[randomChunkIndex];
            var chunkComponent = randomChunk.GetComponent<ChunkComponent>();
            
            var chunkPosition = (Vector3Int)(CenterChunkPosition + direction.ToVector2Int() * MapAsset.ChunkSize);
            var fromBounds = new BoundsInt(Vector3Int.zero, new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize, 1));
            var toBounds = new BoundsInt(chunkPosition, new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize, 1));
            
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
            var chunkPosition = (Vector3Int)(CenterChunkPosition + direction.ToVector2Int() * MapAsset.ChunkSize);
            var bounds = new BoundsInt(chunkPosition, new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize, 1));
            var tiles = new TileBase[MapAsset.ChunkSize * MapAsset.ChunkSize];
            
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
            
            CenterChunkPosition += direction.ToVector2Int() * MapAsset.ChunkSize;
            
            SpawnChunks(directionsToAdd);
            UpdateBounds();
            
        }

        void UpdateBounds()
        {
            var mapBounds = new BoundsInt(
                (Vector3Int)CenterChunkPosition - new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize), 
                new Vector3Int(MapAsset.ChunkSize * 3, MapAsset.ChunkSize * 3, 1));
            mapBounds.ClampToBounds(mapBounds);
            MapBounds = mapBounds;
            
            var centerBounds = new BoundsInt((Vector3Int)CenterChunkPosition, new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize, 1));
            centerBounds.ClampToBounds(centerBounds);
            CenterChunkBounds = centerBounds;
        }
        
        IEnumerable<Tilemap> GetWalkableTilemaps() => MapAsset.TilemapIsWalkable
            .Keys
            .Where(e => MapAsset.TilemapIsWalkable[e])
            .Select(e => Tilemaps[e]);

        public IEnumerable<Tilemap> GetObstacleTilemaps() => MapAsset.TilemapIsWalkable
            .Keys
            .Where(e => !MapAsset.TilemapIsWalkable[e])
            .Select(e => Tilemaps[e]);
    }
}