using System;
using System.Linq;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;

namespace WorkingTitle.Lib.Extensions
{
    public static class Vector2IntExtensions
    {
        public static Direction ToDirection(this Vector2Int value) =>
            DirectionExtensions.CardinalAndInterCardinal
                .DefaultIfEmpty(Direction.None)
                .FirstOrDefault(e => e.ToVector2Int() == value);
    }
}