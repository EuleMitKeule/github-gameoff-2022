using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorkingTitle.Unity.Extensions
{
    public static class TilemapExtensions
    {
        public static BoundsInt GetBounds(this IEnumerable<Tilemap> tilemaps) => 
            tilemaps.Aggregate(new BoundsInt(), (current, tilemap) => current.Encapsulate(tilemap.cellBounds));

        public static IEnumerable<Vector2Int> GetTilePositions(this Tilemap tilemap)
        {
            var positions = new List<Vector2Int>();

            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(position)) continue;
                positions.Add((Vector2Int)position);
            }
            
            return positions;
        }
        
        public static IEnumerable<Vector2Int> GetTilePositions(this IEnumerable<Tilemap> tilemaps) => tilemaps
            .SelectMany(tilemap => tilemap.GetTilePositions())
            .ToList();
    }
}