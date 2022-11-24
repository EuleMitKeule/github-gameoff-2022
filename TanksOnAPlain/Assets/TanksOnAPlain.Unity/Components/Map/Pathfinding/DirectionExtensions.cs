using System.Collections.Generic;
using System.Linq;
using TanksOnAPlain.Unity.Extensions;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Map.Pathfinding
{
    public static class DirectionExtensions
    {
        public static IEnumerable<Direction> All => new[]
        {
            Direction.None,
            Direction.Up,
            Direction.UpLeft,
            Direction.Left,
            Direction.DownLeft,
            Direction.Down,
            Direction.DownRight,
            Direction.Right,
            Direction.UpRight
        };

        public static IEnumerable<Direction> Cardinal => new[]
        {
            Direction.Up,
            Direction.Left,
            Direction.Down,
            Direction.Right
        };

        public static IEnumerable<Direction> CardinalAndInterCardinal => new[]
        {
            Direction.Up,
            Direction.UpLeft,
            Direction.Left,
            Direction.DownLeft,
            Direction.Down,
            Direction.DownRight,
            Direction.Right,
            Direction.UpRight
        };
        
        public static IEnumerable<Direction> InterCardinal => new[]
        {
            Direction.UpLeft,
            Direction.DownLeft,
            Direction.DownRight,
            Direction.UpRight
        };

        public static IEnumerable<Direction> Upwards => new[]
        {
            Direction.Up,
            Direction.UpLeft,
            Direction.UpRight
        };
        
        public static IEnumerable<Direction> Leftwards => new[]
        {
            Direction.UpLeft,
            Direction.Left,
            Direction.DownLeft
        };
        
        public static IEnumerable<Direction> Downwards => new[]
        {
            Direction.Down,
            Direction.DownLeft,
            Direction.DownRight
        };
        
        public static IEnumerable<Direction> Rightwards => new[]
        {
            Direction.UpRight,
            Direction.Right,
            Direction.DownRight
        };
        
        public static Vector2Int ToVector2Int(this Direction direction) => direction switch
        {
            Direction.None => Vector2Int.zero,
            Direction.Up => Vector2Int.up,
            Direction.UpLeft => Vector2Int.up + Vector2Int.left,
            Direction.Left => Vector2Int.left,
            Direction.DownLeft => Vector2Int.down + Vector2Int.left,
            Direction.Down => Vector2Int.down,
            Direction.DownRight => Vector2Int.down + Vector2Int.right,
            Direction.Right => Vector2Int.right,
            Direction.UpRight => Vector2Int.up + Vector2Int.right,
            _ => Vector2Int.zero
        };
        
        public static Vector3Int ToVector3Int(this Direction direction) => direction switch
        {
            Direction.None => Vector3Int.zero,
            Direction.Up => Vector3Int.up,
            Direction.UpLeft => Vector3Int.up + Vector3Int.left,
            Direction.Left => Vector3Int.left,
            Direction.DownLeft => Vector3Int.down + Vector3Int.left,
            Direction.Down => Vector3Int.down,
            Direction.DownRight => Vector3Int.down + Vector3Int.right,
            Direction.Right => Vector3Int.right,
            Direction.UpRight => Vector3Int.up + Vector3Int.right,
            _ => Vector3Int.zero
        };
        
        public static Vector2 ToVector2(this Direction direction) => (direction switch
        {
            Direction.None => Vector2.zero,
            Direction.Up => Vector2.up,
            Direction.UpLeft => Vector2.up + Vector2.left,
            Direction.Left => Vector2.left,
            Direction.DownLeft => Vector2.down + Vector2.left,
            Direction.Down => Vector2.down,
            Direction.DownRight => Vector2.down + Vector2.right,
            Direction.Right => Vector2.right,
            Direction.UpRight => Vector2.up + Vector2.right,
            _ => Vector2.zero
        }).normalized;
        
        public static IEnumerable<Vector2> ToVector2(this IEnumerable<Direction> directions) =>
            directions.Select(e => e.ToVector2());

        public static float ToCost(this Direction direction) =>
            direction.ToVector2Int().magnitude;
        
        public static IEnumerable<Direction> ToOpposite(this IEnumerable<Direction> directions) =>
            directions.Select(e => e.ToOpposite());
        
        public static Direction ToOpposite(this Direction direction) => direction switch
        {
            Direction.None => Direction.None,
            Direction.Up => Direction.Down,
            Direction.UpLeft => Direction.DownRight,
            Direction.Left => Direction.Right,
            Direction.DownLeft => Direction.UpRight,
            Direction.Down => Direction.Up,
            Direction.DownRight => Direction.UpLeft,
            Direction.Right => Direction.Left,
            Direction.UpRight => Direction.DownLeft,
            _ => Direction.None
        };

        public static Direction MoveDirection(this Direction direction, Direction moveDirection)
        {
            var newDirection = (direction.ToVector2Int() + moveDirection.ToVector2Int()).ToDirection();
            
            return newDirection;
        }
    }
}