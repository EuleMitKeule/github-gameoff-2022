using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Map.Pathfinding
{
    public struct PathfindingCell
    {
        public Vector2Int Position { get; }
        public byte BaseCost { get; }
        public bool IsObstacle { get; }
        public float Cost { get; set; }
        public Vector2 Direction { get; set; }

        public PathfindingCell(Vector2Int position, byte baseCost, bool isObstacle, float cost) : this()
        {
            Position = position;
            BaseCost = baseCost;
            IsObstacle = isObstacle;
            Cost = cost;
        }
    }
}