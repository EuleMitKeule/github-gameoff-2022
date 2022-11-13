using System.Collections.Generic;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Components.Map;

namespace WorkingTitle.Unity.Extensions
{
    public static class ChunkComponentExtensions
    {
        public static Direction GetChunk(this Dictionary<Direction, ChunkComponent> chunks, Vector2Int position)
        {
            var direction = Direction.None;
            
            foreach (var (chunkDirection, chunkComponent) in chunks)
            {
                var containsPosition = chunkComponent.Bounds.Contains((Vector3Int)position);

                if (!containsPosition) continue;
                
                direction = chunkDirection;
                break;
            }
            
            return direction;
        }           
    }
}