using System.Linq;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;

namespace WorkingTitle.Lib.Extensions
{
    public static class Vector2Extensions
    {
        public static PathfindingDirection ToPathfindingDirection(this Vector2 value) =>
            PathfindingDirectionExtensions.All
                .DefaultIfEmpty(PathfindingDirection.None)
                .FirstOrDefault(direction => direction.ToVector2() == value);
    }
}