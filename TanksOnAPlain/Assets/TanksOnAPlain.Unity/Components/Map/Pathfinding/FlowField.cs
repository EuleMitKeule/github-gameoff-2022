using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Map.Pathfinding
{
    public class FlowField
    {
        public PathfindingCell TargetCell { get; }
        public Dictionary<Vector2Int, PathfindingCell> Cells { get; } = new();

        public BoundsInt MapBounds { get; }

        public bool IsCostsCalculated { get; private set; }
        public bool IsDirectionsCalculated { get; private set; }

        public FlowField(
            Vector2Int targetPosition,
            List<Vector2Int> obstaclePositions,
            BoundsInt mapBounds)
        {
            if (obstaclePositions == null)
            {
                throw new ArgumentNullException(nameof(obstaclePositions));
            }

            if (obstaclePositions.Any(e =>
                e.x < mapBounds.xMin || e.x >= mapBounds.xMax ||
                e.y < mapBounds.yMin || e.y >= mapBounds.yMax))
            {
                throw new ArgumentException(
                    $"Items in '{nameof(obstaclePositions)}' must be between {mapBounds.min} and {mapBounds.max}",
                    nameof(obstaclePositions));
            }

            if (targetPosition.x < mapBounds.xMin || targetPosition.x >= mapBounds.xMax ||
                targetPosition.y < mapBounds.yMin || targetPosition.y >= mapBounds.yMax)
            {
                throw new ArgumentException($"'{nameof(targetPosition)}' is out of map bounds.", nameof(targetPosition));
            }

            if (mapBounds.size.x <= 0 || mapBounds.size.y <= 0)
            {
                throw new ArgumentException($"'{nameof(mapBounds)}' size must be greater than zero.", nameof(mapBounds));
            }
            
            for (var x = mapBounds.xMin; x < mapBounds.xMax; x++)
            {
                for (var y = mapBounds.yMin; y < mapBounds.yMax; y++)
                {
                    var position = new Vector2Int(x, y);
                    var isTargetCell = position == targetPosition;
                    var baseCost = isTargetCell ? 0 : 1;
                    var isObstacle = obstaclePositions.Contains(position);
                    var cost = isTargetCell ? 0 : ushort.MaxValue;

                    var cell = new PathfindingCell
                    {
                        BaseCost = (byte)baseCost,
                        IsObstacle = isObstacle,
                        Position = position,
                        Cost = (ushort)cost

                    };
                    Cells.Add(position, cell);
                }
            }

            TargetCell = Cells[targetPosition];
            MapBounds = mapBounds;
        }

        public void CalcCosts()
        {
            var queue = new Queue<PathfindingCell>();
            queue.Enqueue(TargetCell);

            var sqrtTwo = (float)Math.Sqrt(2);

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                var neighborCells = GetNeighborCells(cell.Position, false);

                for (int neighborX = -1; neighborX < 2; neighborX++)
                {
                    for (int neighborY = -1; neighborY < 2; neighborY++)
                    {
                        if (neighborX == 0 && neighborY == 0) continue;

                        var neighborPosition = cell.Position + new Vector2Int(neighborX, neighborY);
                        
                        if (!neighborCells.ContainsKey(neighborPosition)) continue;
                        var neighborCell = neighborCells[neighborPosition];

                        if (neighborCell.IsObstacle) continue;

                        var isInterCardinal = neighborX != 0 && neighborY != 0;

                        if (isInterCardinal)
                        {
                            var horizontalNeighborPosition = cell.Position + new Vector2Int(neighborX, 0);
                            var verticalNeighborPosition = cell.Position + new Vector2Int(0, neighborY);
                            
                            if (!neighborCells.ContainsKey(horizontalNeighborPosition) ||
                                !neighborCells.ContainsKey(verticalNeighborPosition)) 
                                continue;
                            
                            if (neighborCells[horizontalNeighborPosition].IsObstacle || 
                                neighborCells[verticalNeighborPosition].IsObstacle)
                                continue;
                        }

                        var baseCost = (float)neighborCell.BaseCost;
                        if (isInterCardinal) baseCost *= sqrtTwo;
                        
                        var neighborCost = baseCost + cell.Cost;

                        if (neighborCost >= neighborCell.Cost) continue;

                        neighborCell.Cost = neighborCost;
                        queue.Enqueue(neighborCell);
                    }
                }
            }

            IsCostsCalculated = true;
        }

        public void CalcDirections()
        {
            for (var x = MapBounds.xMin; x < MapBounds.xMax; x++)
            {
                for (var y = MapBounds.yMin; y < MapBounds.yMax; y++)
                {
                    var position = new Vector2Int(x, y);
                    var cell = Cells[position];

                    if (cell.IsObstacle) continue;

                    var neighborCells = GetNeighborCells(cell.Position, false);
                    var bestCost = cell.Cost;

                    for (int neighborX = -1; neighborX < 2; neighborX++)
                    {
                        for (int neighborY = -1; neighborY < 2; neighborY++)
                        {
                            if (neighborX == 0 && neighborY == 0) continue;
                            
                            var neighborPosition = cell.Position + new Vector2Int(neighborX, neighborY);
                            
                            if (!neighborCells.ContainsKey(neighborPosition)) continue;
                            var neighborCell = neighborCells[neighborPosition];

                            if (neighborCell.IsObstacle || neighborCell.Cost >= bestCost) continue;

                            var isInterCardinal = neighborX != 0 && neighborY != 0;

                            if (isInterCardinal)
                            {
                                var horizontalNeighborPosition = cell.Position + new Vector2Int(neighborX, 0);
                                var verticalNeighborPosition = cell.Position + new Vector2Int(0, neighborY);
                            
                                if (!neighborCells.ContainsKey(horizontalNeighborPosition) ||
                                    !neighborCells.ContainsKey(verticalNeighborPosition)) 
                                    continue;
                            
                                if (neighborCells[horizontalNeighborPosition].IsObstacle || 
                                    neighborCells[verticalNeighborPosition].IsObstacle)
                                    continue;
                            }
                            
                            bestCost = neighborCell.Cost;
                            cell.Direction = new Vector2(neighborX, neighborY).normalized;
                        }
                    }
                }
            }

            IsDirectionsCalculated = true;
        }

        Dictionary<Vector2Int, PathfindingCell> GetNeighborCells(Vector2Int position, bool skipInterCardinal)
        {
            var neighborCells = new Dictionary<Vector2Int, PathfindingCell>();

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (skipInterCardinal && x != 0 && y != 0) continue;

                    var neighborPositionX = position.x + x;
                    var neighborPositionY = position.y + y;

                    if (neighborPositionX < MapBounds.xMin || neighborPositionX >= MapBounds.xMax ||
                        neighborPositionY < MapBounds.yMin || neighborPositionY >= MapBounds.yMax)
                    {
                        continue;
                    }

                    var neighborPosition = new Vector2Int(neighborPositionX, neighborPositionY);
                    var neighborCell = Cells[neighborPosition];

                    neighborCells.Add(neighborPosition, neighborCell);
                }
            }

            return neighborCells;
        }
    }
}