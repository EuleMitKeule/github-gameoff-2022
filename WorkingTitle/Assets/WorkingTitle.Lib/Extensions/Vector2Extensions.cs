using System.Linq;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;

namespace WorkingTitle.Lib.Extensions
{
    public static class Vector2Extensions
    {
        public static Direction ToPathfindingDirection(this Vector2 value) =>
            DirectionExtensions.All
                .DefaultIfEmpty(Direction.None)
                .FirstOrDefault(direction => direction.ToVector2() == value);
    }
}