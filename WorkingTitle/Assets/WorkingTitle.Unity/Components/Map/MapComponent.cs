using System;
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
        public BoundsInt ChunkBounds { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        Vector2Int ChunkIndex { get; set; }

        [ShowInInspector]
        [ReadOnly]
        public Vector2Int ChunkPosition { get; set; }

        List<Dictionary<TilemapType, TileBase[]>> ChunkTiles { get; } = new();
        Dictionary<Vector2Int, Dictionary<TilemapType, TileBase[]>> Chunks { get; } = new();
        
        Grid Grid { get; set; }
        EntityComponent PlayerEntityComponent { get; set; }

        void Awake()
        {
            Grid = GetComponent<Grid>();
            MapSize = new Vector2Int(MapAsset.ChunkSize, MapAsset.ChunkSize);

            CacheChunks();
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
            
            ChooseChunks(DirectionExtensions.All.Select(e => e.ToVector2Int()).ToList());
            SpawnChunk(Vector2Int.zero);
            
            if (MapAsset.ChunkSize % 2 == 0)
            {
                var relativeCellPosition = new Vector2Int(MapAsset.ChunkSize / 2, MapAsset.ChunkSize / 2);
                ExtendMap(Direction.Up, relativeCellPosition);
                ExtendMap(Direction.Right, relativeCellPosition);
            }
            
            UpdateBounds();
        }

        void CacheChunks()
        {
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

        void SpawnChunk(Vector2Int chunkIndex)
        {
            if (!Chunks.ContainsKey(chunkIndex)) return;

            var chunk = Chunks[chunkIndex];
            var chunkPosition = (Vector3Int)(ChunkPosition + chunkIndex * MapAsset.ChunkSize);
            var toBounds = new BoundsInt(chunkPosition, new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize, 1));
            
            foreach (var (sourceTilemapType, sourceTiles) in chunk)
            {
                if (!Tilemaps.Keys.Contains(sourceTilemapType)) continue;
                
                var destinationTilemap = Tilemaps[sourceTilemapType];
                
                destinationTilemap.SetTilesBlock(toBounds, sourceTiles);
            }
        }

        void ChooseChunk(Vector2Int chunkIndex)
        {
            if (Chunks.ContainsKey(chunkIndex)) return;
                
            var randomChunkIndex = Random.Range(0, ChunkTiles.Count);
            var randomChunkTiles = ChunkTiles[randomChunkIndex];
                
            Chunks.Add(chunkIndex, randomChunkTiles);
        }
        
        void ChooseChunks(IEnumerable<Vector2Int> chunkIndices)
        {
            foreach (var chunkIndex in chunkIndices)
            {
                ChooseChunk(chunkIndex);
            }
        }

        Vector2Int PositionToChunkIndex(Vector2Int position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / (float) MapAsset.ChunkSize),
                Mathf.FloorToInt(position.y / (float) MapAsset.ChunkSize)
            );
        }
        
        int Modulo(int x, int m)
        {
            return (x % m + m) % m;
        }
        
        void ExtendMap(Direction cardinalDirection, Vector2Int? playerCellPosition = null)
        {
            if (cardinalDirection == Direction.None) return;
            
            playerCellPosition ??= new Vector2Int(
                PlayerEntityComponent.CellPosition.x,
                PlayerEntityComponent.CellPosition.y);

            var chunkDirections = cardinalDirection switch
            {
                _ when DirectionExtensions.Upwards.Contains(cardinalDirection) => DirectionExtensions.Upwards.ToList(),
                _ when DirectionExtensions.Leftwards.Contains(cardinalDirection) => DirectionExtensions.Leftwards.ToList(),
                _ when DirectionExtensions.Downwards.Contains(cardinalDirection) => DirectionExtensions.Downwards.ToList(),
                _ when DirectionExtensions.Rightwards.Contains(cardinalDirection) => DirectionExtensions.Rightwards.ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(cardinalDirection), cardinalDirection, null)
            };
            
            var movedPlayerCellPosition = playerCellPosition.Value + cardinalDirection.ToVector2Int() * MapAsset.ChunkSize / 2;
            var baseChunkIndex = PositionToChunkIndex(movedPlayerCellPosition);

            var chunkIndices = new List<Vector2Int>();
            
            foreach (var chunkDirection in chunkDirections)
            {
                var chunkIndex = baseChunkIndex + chunkDirection.ToVector2Int() - cardinalDirection.ToVector2Int();
                chunkIndices.Add(chunkIndex);
            }
            
            foreach (var chunkIndex in chunkIndices)
            {
                var chunkPosition = chunkIndex * MapAsset.ChunkSize;

                var tilesToSpawn = new List<TileBase>();
                var tilePositionsToSpawn = new List<Vector3Int>();
                
                var relativeX = Modulo(
                    Modulo(playerCellPosition.Value.x, MapAsset.ChunkSize) + cardinalDirection.ToVector2Int().x * MapAsset.ChunkSize / 2,
                    MapAsset.ChunkSize);
                var relativeY = Modulo(
                    Modulo(playerCellPosition.Value.y, MapAsset.ChunkSize) +
                    cardinalDirection.ToVector2Int().y * MapAsset.ChunkSize / 2,
                    MapAsset.ChunkSize);
                
                foreach (var tilemapType in Tilemaps.Keys)
                {
                    if (cardinalDirection is Direction.Right or Direction.Left)
                    {
                        for (relativeY = 0; relativeY < MapAsset.ChunkSize; relativeY++)
                        {
                            var tileIndex = relativeX * MapAsset.ChunkSize + relativeY;
                            var tile = Chunks[chunkIndex][tilemapType][tileIndex];

                            if (!tile) continue;
                            
                            var relativePosition = new Vector3Int(relativeX, relativeY);
                            var cellPosition = (Vector3Int)chunkPosition + relativePosition;
                            
                            tilesToSpawn.Add(tile);
                            tilePositionsToSpawn.Add(cellPosition);
                        }
                        
                        Debug.Log($"SPAWNING COL AT x = {relativeX} of chunk with index {chunkIndex}");
                    }
                    else if (cardinalDirection is Direction.Up or Direction.Down)
                    {
                        
                        for (relativeX = 0; relativeX < MapAsset.ChunkSize; relativeX++)
                        {
                            var tileIndex = relativeX * MapAsset.ChunkSize + relativeY;
                            var tile = Chunks[chunkIndex][tilemapType][tileIndex];
                            
                            if (!tile) continue;
                            
                            var relativePosition = new Vector3Int(relativeX, relativeY);
                            var position = (Vector3Int)chunkPosition + relativePosition;

                            tilesToSpawn.Add(tile);
                            tilePositionsToSpawn.Add(position);
                        }
                    }

                    var destinationTilemap = Tilemaps[tilemapType];
                    destinationTilemap.SetTiles(tilePositionsToSpawn.ToArray(), tilesToSpawn.ToArray());
                    
                    tilesToSpawn.Clear();
                    tilePositionsToSpawn.Clear();
                }
            }
        }

        void OnChunkChanged(object sender, Direction direction)
        {
            if (direction == Direction.None) return;
            
            ChunkIndex += direction.ToVector2Int();
            ChunkPosition += direction.ToVector2Int() * MapAsset.ChunkSize;

            var directionsToAdd = direction switch
            {
                _ when DirectionExtensions.Upwards.Contains(direction) => DirectionExtensions.Upwards.ToList(),
                _ when DirectionExtensions.Leftwards.Contains(direction) => DirectionExtensions.Leftwards.ToList(),
                _ when DirectionExtensions.Downwards.Contains(direction) => DirectionExtensions.Downwards.ToList(),
                _ when DirectionExtensions.Rightwards.Contains(direction) => DirectionExtensions.Rightwards.ToList(),
                _ => new List<Direction>()
            };
            
            var chunkIndicesToAdd = directionsToAdd.Select(d => ChunkIndex + d.ToVector2Int()).ToList();
            
            ChooseChunks(chunkIndicesToAdd);
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
                (Vector3Int)ChunkPosition - new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize), 
                new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize, 1));
            mapBounds.ClampToBounds(mapBounds);
            MapBounds = mapBounds;
            
            var chunkBounds = new BoundsInt((Vector3Int)ChunkPosition, new Vector3Int(MapAsset.ChunkSize, MapAsset.ChunkSize, 1));
            chunkBounds.ClampToBounds(chunkBounds);
            ChunkBounds = chunkBounds;
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