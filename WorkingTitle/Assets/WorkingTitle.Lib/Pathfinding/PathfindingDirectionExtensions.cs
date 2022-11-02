using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorkingTitle.Lib.Pathfinding
{
    public static class PathfindingDirectionExtensions
    {
        public static IEnumerable<PathfindingDirection> All => new[]
        {
            PathfindingDirection.Up,
            PathfindingDirection.UpLeft,
            PathfindingDirection.Left,
            PathfindingDirection.DownLeft,
            PathfindingDirection.Down,
            PathfindingDirection.DownRight,
            PathfindingDirection.Right,
            PathfindingDirection.UpRight
        };
        
        public static Vector2Int ToVector2Int(this PathfindingDirection direction) => direction switch
        {
            PathfindingDirection.None => Vector2Int.zero,
            PathfindingDirection.Up => Vector2Int.up,
            PathfindingDirection.UpLeft => Vector2Int.up + Vector2Int.left,
            PathfindingDirection.Left => Vector2Int.left,
            PathfindingDirection.DownLeft => Vector2Int.down + Vector2Int.left,
            PathfindingDirection.Down => Vector2Int.down,
            PathfindingDirection.DownRight => Vector2Int.down + Vector2Int.right,
            PathfindingDirection.Right => Vector2Int.right,
            PathfindingDirection.UpRight => Vector2Int.up + Vector2Int.right,
            _ => Vector2Int.zero
        };
        
        public static Vector2 ToVector2(this PathfindingDirection direction) => (direction switch
        {
            PathfindingDirection.None => Vector2.zero,
            PathfindingDirection.Up => Vector2.up,
            PathfindingDirection.UpLeft => Vector2.up + Vector2.left,
            PathfindingDirection.Left => Vector2.left,
            PathfindingDirection.DownLeft => Vector2.down + Vector2.left,
            PathfindingDirection.Down => Vector2.down,
            PathfindingDirection.DownRight => Vector2.down + Vector2.right,
            PathfindingDirection.Right => Vector2.right,
            PathfindingDirection.UpRight => Vector2.up + Vector2.right,
            _ => Vector2.zero
        }).normalized;
        
        public static IEnumerable<Vector2> ToVector2(this IEnumerable<PathfindingDirection> directions) =>
            directions.Select(e => e.ToVector2());

        public static float ToCost(this PathfindingDirection direction) =>
            direction.ToVector2Int().magnitude;
    }
}