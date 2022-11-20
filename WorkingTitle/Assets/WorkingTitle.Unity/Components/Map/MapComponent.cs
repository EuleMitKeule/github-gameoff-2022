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
using WorkingTitle.Unity.Extensions;
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
        Camera Camera { get; set; }

        void Awake()
        {
            Grid = GetComponent<Grid>();
            Camera = FindObjectOfType<Camera>();
            MapSize = new Vector2Int(MapAsset.ViewDistance * 2, MapAsset.ViewDistance * 2);

            CacheChunks();
        }

        void Start()
        {
            PlayerEntityComponent = 
                FindObjectOfType<PlayerTankComponent>()
                .GetComponent<EntityComponent>();
            
            if (PlayerEntityComponent)
            {
                PlayerEntityComponent.ChunkChanged += OnChunkChanged;
                PlayerEntityComponent.CellPositionChanged += OnCellPositionChanged; 
            }

            var centerPosition = new Vector2Int(MapAsset.ChunkSize / 2, MapAsset.ChunkSize / 2);
            
            PlayerEntityComponent.SetPosition(centerPosition);
            Camera.transform.position = centerPosition.ToWorld();
            
            UpdateMap(true);
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

        void ChooseChunk(Vector2Int chunkIndex)
        {
            if (Chunks.ContainsKey(chunkIndex)) return;
                
            var randomChunkIndex = Random.Range(0, ChunkTiles.Count);
            var randomChunkTiles = ChunkTiles[randomChunkIndex];
                
            Chunks.Add(chunkIndex, randomChunkTiles);
        }
        
        Vector2Int PositionToChunkIndex(Vector3Int position)
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

        void OnChunkChanged(object sender, Direction direction)
        {
            if (direction == Direction.None) return;
            
            ChunkIndex += direction.ToVector2Int();
            ChunkPosition += direction.ToVector2Int() * MapAsset.ChunkSize;
        }

        void OnCellPositionChanged(object sender, CellPositionChangedEventArgs e)
        {
            UpdateMap();
            
            UpdateBounds();
        }

        void UpdateMap(bool ignoreMinDistance = false)
        {
            var tilesToSpawn = new List<TileBase>();
            var tilePositionsToSpawn = new List<Vector3Int>();
            
            foreach (var (tilemapType, destinationTilemap) in Tilemaps)
            {
                for (int x = -MapAsset.ViewDistance - 1; x < MapAsset.ViewDistance + 2; x++)
                {
                    for (int y = -MapAsset.ViewDistance - 1; y < MapAsset.ViewDistance + 2; y++)
                    {
                        var relativePosition = new Vector3Int(x, y);

                        var absoluteX = Mathf.Abs(relativePosition.x);
                        var absoluteY = Mathf.Abs(relativePosition.y);
                        
                        if ((absoluteX < MapAsset.ViewDistance - 1 &&
                            absoluteY < MapAsset.ViewDistance - 1) &&
                            !ignoreMinDistance)
                        {
                            continue;
                        }

                        if (absoluteX > MapAsset.ViewDistance + 1 ||
                            absoluteY > MapAsset.ViewDistance + 1)
                        {
                            continue;
                        }

                        var position = (Vector3Int)PlayerEntityComponent.CellPosition;
                        var cellPosition = position + relativePosition;
                        var hasTile = destinationTilemap.HasTile(cellPosition);
                        
                        if (absoluteX > MapAsset.ViewDistance ||
                            absoluteY > MapAsset.ViewDistance)
                        {
                            if (!hasTile) continue;
                            
                            tilesToSpawn.Add(null);
                            tilePositionsToSpawn.Add(cellPosition);
                            continue;
                        }

                        if (hasTile) continue;
                        
                        var relativeChunkPosition = new Vector2Int(
                            Modulo(cellPosition.x, MapAsset.ChunkSize),
                            Modulo(cellPosition.y, MapAsset.ChunkSize));
                        
                        var chunkIndex = PositionToChunkIndex(cellPosition);
                        
                        if (!Chunks.ContainsKey(chunkIndex)) ChooseChunk(chunkIndex);
                        
                        var chunk = Chunks[chunkIndex];
                        var tileIndex = relativeChunkPosition.y * MapAsset.ChunkSize + relativeChunkPosition.x;
                        var tile = chunk[tilemapType][tileIndex];
                        
                        if (!tile) continue;
                        
                        tilesToSpawn.Add(tile);
                        tilePositionsToSpawn.Add(cellPosition);
                    }
                }
                
                destinationTilemap.SetTiles(tilePositionsToSpawn.ToArray(), tilesToSpawn.ToArray());
                destinationTilemap.CompressBounds();
                
                tilesToSpawn.Clear();
                tilePositionsToSpawn.Clear();
            }
        }

        void UpdateBounds()
        {
            var mapBounds = new BoundsInt(
                (Vector3Int)PlayerEntityComponent.CellPosition - new Vector3Int(MapAsset.ViewDistance, MapAsset.ViewDistance), 
                new Vector3Int(MapAsset.ViewDistance * 2, MapAsset.ViewDistance * 2, 1));
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