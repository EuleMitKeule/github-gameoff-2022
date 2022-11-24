using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TanksOnAPlain.Unity.Extensions
{
    public static class TilemapExtensions
    {
        public static BoundsInt GetBounds(this IEnumerable<Tilemap> tilemaps, Vector3Int position = default)
        {
            var totalBounds = new BoundsInt
            {
                position = position
            };

            foreach (var tilemap in tilemaps)
            {
                totalBounds = totalBounds.Encapsulate(tilemap.cellBounds);
            }

            return totalBounds;
        }
            
            // => 
            // tilemaps.Aggregate(new BoundsInt(), (current, tilemap) => current.Encapsulate(tilemap.cellBounds));

        public static IEnumerable<Vector2Int> GetTilePositions(this Tilemap tilemap, BoundsInt? bounds)
        {
            bounds ??= tilemap.cellBounds;
            var positions = new List<Vector2Int>();

            for (int x = bounds.Value.xMin; x < bounds.Value.xMax; x++)
            {
                for (int y = bounds.Value.yMin; y < bounds.Value.yMax; y++)
                {
                    var position = new Vector3Int(x, y);
                    
                    if (!tilemap.HasTile(position)) continue;
                    positions.Add((Vector2Int)position);
                }
            }
            
            return positions;
        }
        
        public static IEnumerable<Vector2Int> GetTilePositions(this IEnumerable<Tilemap> tilemaps, BoundsInt? bounds) => tilemaps
            .SelectMany(tilemap => tilemap.GetTilePositions(bounds))
            .ToList();

        public static void MoveTiles(this Tilemap tilemap, BoundsInt fromBounds, BoundsInt toBounds)
        {
            if (fromBounds == toBounds) return;
            
            var tiles = tilemap.GetTilesBlock(fromBounds);
            tilemap.SetTilesBlock(toBounds, tiles);
            tilemap.SetTilesBlock(fromBounds, new TileBase[tiles.Length]);
            tilemap.CompressBounds();
        }
    }
}