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
    }
}