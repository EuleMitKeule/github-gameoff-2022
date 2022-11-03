using UnityEngine;

namespace WorkingTitle.Lib.Pathfinding
{
    public class PathfindingCell
    {
        public Vector2Int Position { get; set; }
        public byte BaseCost { get; set; }
        public bool IsObstacle { get; set; }
        public float Cost { get; set; } = float.PositiveInfinity;
        public PathfindingDirection Direction { get; set; }
    }
}