using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Tilemaps;
using WorkingTitle.Lib.Extensions;
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
        Vector2Int CenterChunkIndex { get; set; }

        [ShowInInspector]
        [ReadOnly]
        Vector2Int CenterChunkPosition { get; set; }

        List<Dictionary<TilemapType, TileBase[]>> ChunkTiles { get; } = new();
        Dictionary<Vector2Int, Dictionary<TilemapType, TileBase[]>> Chunks { get; } = new();
        
        Grid Grid { get; set; }
        EntityComponent PlayerEntityComponent { get; set; }

        void Awake()
        {
            Grid = GetComponent<Grid>();
            MapSize = new Vector2Int(3 * MapAsset.ChunkSize, 3 * MapAsset.ChunkSize);

            var bounds = new BoundsInt(Vector3Int.zero, new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize, 1));
            
            foreach (var chunkPrefab in MapAsset.ChunkPrefabs)
            {
                var chunkComponent = chunkPrefab.GetComponent<ChunkComponent>();
                var chunkTilesEntry = new Dictionary<TilemapType, TileBase[]>();
                
                foreach (var tilemapType in Tilemaps.Keys)
                {
                    if (!chunkComponent.Tilemaps.ContainsKey(tilemapType)) continue;

                    var tilemap = chunkComponent.Tilemaps[tilemapType];
                    var chunkTiles = new TileBase[MapAsset.ChunkSize * MapAsset.ChunkSize];
                    tilemap.GetTilesBlockNonAlloc(bounds, chunkTiles);
                    
                    chunkTilesEntry.Add(tilemapType, chunkTiles);
                }
                
                ChunkTiles.Add(chunkTilesEntry);
            }
        }

        void Start()
        {
            PlayerEntityComponent = 
                GetComponentInChildren<PlayerComponent>()
                .GetComponent<EntityComponent>();
            
            if (PlayerEntityComponent)
            {
                PlayerEntityComponent.ChunkChanged += OnChunkChanged;
                PlayerEntityComponent.CellPositionChanged += OnCellPositionChanged; 
            }

            var chunkDirections = DirectionExtensions.All.Select(e => e.ToVector2Int()).ToList();
            chunkDirections.AddRange(DirectionExtensions.Upwards.Select(e => e.ToVector2Int() + Vector2Int.up).ToList());
            chunkDirections.AddRange(DirectionExtensions.Leftwards.Select(e => e.ToVector2Int() + Vector2Int.left).ToList());
            chunkDirections.AddRange(DirectionExtensions.Downwards.Select(e => e.ToVector2Int() + Vector2Int.down).ToList());
            chunkDirections.AddRange(DirectionExtensions.Rightwards.Select(e => e.ToVector2Int() + Vector2Int.right).ToList());
            
            ChooseChunks(chunkDirections);
            SpawnChunks(DirectionExtensions.All);
            if (MapAsset.ChunkSize % 2 == 0)
            {
                ExtendMap(Direction.Up);
                ExtendMap(Direction.Right);
            }
            UpdateBounds();
        }

        void SpawnChunk(Direction direction)
        {
            var chunkIndex = CenterChunkIndex + direction.ToVector2Int();
            if (!Chunks.ContainsKey(chunkIndex)) return;

            var chunk = Chunks[chunkIndex];
            
            
            var chunkPosition = (Vector3Int)(CenterChunkPosition + direction.ToVector2Int() * MapAsset.ChunkSize);
            var toBounds = new BoundsInt(chunkPosition, new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize, 1));
            
            foreach (var (sourceTilemapType, sourceTiles) in chunk)
            {
                if (!Tilemaps.Keys.Contains(sourceTilemapType)) continue;
                
                var destinationTilemap = Tilemaps[sourceTilemapType];
                
                destinationTilemap.SetTilesBlock(toBounds, sourceTiles);
            }
        }

        void SpawnChunks(IEnumerable<Direction> directions)
        {
            foreach (var direction in directions)
            {
                SpawnChunk(direction);
            }
        }

        void ChooseChunk(Vector2Int direction)
        {
            var chunkPosition = CenterChunkIndex + direction;
            if (Chunks.ContainsKey(chunkPosition)) return;
                
            var randomChunkIndex = Random.Range(0, ChunkTiles.Count);
            var randomChunkTiles = ChunkTiles[randomChunkIndex];
                
            Chunks.Add(chunkPosition, randomChunkTiles);
        }
        
        void ChooseChunks(IEnumerable<Direction> directions)
        {
            foreach (var direction in directions)
            {
                ChooseChunk(direction.ToVector2Int());
            }
        }
        
        void ChooseChunks(IEnumerable<Vector2Int> directions)
        {
            foreach (var direction in directions)
            {
                ChooseChunk(direction);
            }
        }

        void ExtendMap(Direction direction)
        {
            if (direction == Direction.None) return;
            
            var relativeCellPosition = new Vector2Int(
                PlayerEntityComponent.CellPosition.x % MapAsset.ChunkSize, 
                PlayerEntityComponent.CellPosition.y % MapAsset.ChunkSize);

            foreach (var tilemapType in Tilemaps.Keys)
            {
                var chunkIndex = CenterChunkIndex + direction.ToVector2Int() * 2;
                var chunkPosition = chunkIndex * MapAsset.ChunkSize;
                var tilesToSpawn = new List<TileBase>();
                var tilePositionsToSpawn = new List<Vector3Int>();
                var x = 0;
                var y = 0;

                switch (direction)
                {
                    case Direction.Left:
                    case Direction.Right:
                        tilesToSpawn.AddRange(Chunks[chunkIndex][tilemapType]
                        [(relativeCellPosition.x * MapAsset.ChunkSize)..
                            (relativeCellPosition.x * MapAsset.ChunkSize + MapAsset.ChunkSize)]);

                        x = relativeCellPosition.x - MapAsset.ChunkSize * direction.ToVector2Int().x / 2;

                        for (y = 0; y < MapAsset.ChunkSize; y++)
                        {
                            var position = (Vector3Int) chunkPosition + new Vector3Int(x, y);
                            tilePositionsToSpawn.Add(position);
                        }

                        break;

                    case Direction.Up:
                    case Direction.Down:
                        y = relativeCellPosition.y - MapAsset.ChunkSize * direction.ToVector2Int().y / 2;

                        for (x = 0; x < MapAsset.ChunkSize; x++)
                        {
                            var tileIndex = x * MapAsset.ChunkSize + relativeCellPosition.y;
                            var tile = Chunks[chunkIndex][tilemapType][tileIndex];
                            tilesToSpawn.Add(tile);

                            var position = (Vector3Int) chunkPosition + new Vector3Int(x, y);
                            tilePositionsToSpawn.Add(position);
                        }

                        break;
                }

                var destinationTilemap = Tilemaps[tilemapType];
                destinationTilemap.SetTiles(tilePositionsToSpawn.ToArray(), tilesToSpawn.ToArray());
            }
        }

        void OnChunkChanged(object sender, Direction direction)
        {
            if (direction == Direction.None) return;
            
            var directionsToAdd = new List<Vector2Int>();
            switch (direction)
            {
                case Direction.Up:
                    directionsToAdd.AddRange(DirectionExtensions.Upwards
                        .Select(e => e.ToVector2Int() + Vector2Int.up * 2).ToList());
                    directionsToAdd.Add(Direction.UpLeft.ToVector2Int() * 2);
                    directionsToAdd.Add(Direction.UpRight.ToVector2Int() * 2);
                    break;
                case Direction.Left:
                    directionsToAdd.AddRange(DirectionExtensions.Leftwards
                        .Select(e => e.ToVector2Int() + Vector2Int.left * 2).ToList());
                    directionsToAdd.Add(Direction.UpLeft.ToVector2Int() * 2);
                    directionsToAdd.Add(Direction.DownLeft.ToVector2Int() * 2);
                    break;
                case Direction.Down:
                    directionsToAdd.AddRange(DirectionExtensions.Downwards
                        .Select(e => e.ToVector2Int() + Vector2Int.down * 2).ToList());
                    directionsToAdd.Add(Direction.DownLeft.ToVector2Int() * 2);
                    directionsToAdd.Add(Direction.DownRight.ToVector2Int() * 2);
                    break;
                case Direction.Right:
                    directionsToAdd.AddRange(DirectionExtensions.Rightwards
                        .Select(e => e.ToVector2Int() + Vector2Int.right * 2).ToList());
                    directionsToAdd.Add(Direction.DownRight.ToVector2Int() * 2);
                    directionsToAdd.Add(Direction.UpRight.ToVector2Int() * 2);
                    break;
            }
            
            ChooseChunks(directionsToAdd);
            
            CenterChunkIndex += direction.ToVector2Int();
            CenterChunkPosition += direction.ToVector2Int() * MapAsset.ChunkSize;
        }

        void OnCellPositionChanged(object sender, CellPositionChangedEventArgs e)
        {
            var direction = (e.NewCellPosition - e.OldCellPosition).ToDirection();
            
            ExtendMap(direction);
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