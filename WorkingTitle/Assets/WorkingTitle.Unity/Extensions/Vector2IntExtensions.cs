using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorkingTitle.Unity.Extensions
{
    public static class Vector2IntExtensions
    {
        public static Vector2Int ToPositive(this Vector2Int position, BoundsInt bounds) =>
            new (position.x - bounds.xMin, position.y - bounds.yMin);
        
        public static IEnumerable<Vector2Int> ToPositive(this IEnumerable<Vector2Int> positions, BoundsInt bounds) => positions
            .Select(position => position.ToPositive(bounds));
        
        public static Vector2 ToWorld(this Vector2Int position) =>
            new (position.x + 0.5f, position.y + 0.5f);
    }
}