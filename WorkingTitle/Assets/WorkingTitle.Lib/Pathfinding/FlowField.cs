using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorkingTitle.Lib.Pathfinding
{
    public class FlowField
    {
        public PathfindingCell TargetCell { get; }
        public PathfindingCell[][] Cells { get; }

        public Vector2Int GridSize { get; }

        public bool IsCostsCalculated { get; private set; }
        public bool IsDirectionsCalculated { get; private set; }

        public FlowField(
            Vector2Int targetPosition,
            List<Vector2Int> obstaclePositions,
            Vector2Int gridSize)
        {
            if (obstaclePositions == null)
            {
                throw new ArgumentNullException(nameof(obstaclePositions));
            }

            if (obstaclePositions.Any(e =>
                e.x < 0 || e.x >= gridSize.x ||
                e.y < 0 || e.y >= gridSize.y))
            {
                throw new ArgumentException(
                    $"Items in '{nameof(obstaclePositions)}' must be between (0,0) and {gridSize}",
                    nameof(obstaclePositions));
            }

            if (targetPosition.x < 0 || targetPosition.x >= gridSize.x ||
                targetPosition.y < 0 || targetPosition.y >= gridSize.y)
            {
                throw new ArgumentException($"'{nameof(targetPosition)}' is out of grid bounds.", nameof(targetPosition));
            }

            if (gridSize.x <= 0 || gridSize.y <= 0)
            {
                throw new ArgumentException($"'{nameof(gridSize)}' must be greater than zero.", nameof(gridSize));
            }
            
            Cells = new PathfindingCell[gridSize.x][];
            for (var i = 0; i < gridSize.x; i++)
            {
                Cells[i] = new PathfindingCell[gridSize.y];
            }
            
            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
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
                    Cells[x][y] = cell;
                }
            }

            TargetCell = Cells[targetPosition.x][targetPosition.y];
            GridSize = gridSize;
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

                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if (x == 0 && y == 0) continue;

                        var shiftedX = x + 1;
                        var shiftedY = y + 1;
                        var neighborCell = neighborCells[3 * shiftedX + shiftedY];

                        if (neighborCell is null || neighborCell.IsObstacle) continue;

                        var isInterCardinal = x != 0 && y != 0;

                        if (isInterCardinal &&
                            ((neighborCells[shiftedX + 1]?.IsObstacle ?? true) || (neighborCells[3 + shiftedY]?.IsObstacle ?? true)))
                        {
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
            for (var x = 0; x < GridSize.x; x++)
            {
                for (var y = 0; y < GridSize.y; y++)
                {
                    var cell = Cells[x][y];

                    if (cell.IsObstacle) continue;

                    var neighborCells = GetNeighborCells(cell.Position, false);
                    var bestCost = cell.Cost;

                    for (int neighborX = -1; neighborX < 2; neighborX++)
                    {
                        for (int neighborY = -1; neighborY < 2; neighborY++)
                        {
                            if (neighborX == 0 && neighborY == 0) continue;

                            var shiftedX = neighborX + 1;
                            var shiftedY = neighborY + 1;
                            
                            var neighborCell = neighborCells[3 * shiftedX + shiftedY];

                            if (neighborCell is null || neighborCell.IsObstacle || neighborCell.Cost >= bestCost) continue;

                            var isInterCardinal = neighborX == 0 && neighborY == 0;

                            if (isInterCardinal &&
                                ((neighborCells[shiftedX + 1]?.IsObstacle ?? true) || (neighborCells[3 + shiftedY]?.IsObstacle ?? true)))
                            {
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

        PathfindingCell[] GetNeighborCells(Vector2Int position, bool skipInterCardinal)
        {
            var neighborCells = new PathfindingCell[9];

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (skipInterCardinal && x != 0 && y != 0) continue;

                    var neighborPositionX = position.x + x;
                    var neighborPositionY = position.y + y;

                    if (neighborPositionX < 0 || neighborPositionX >= GridSize.x ||
                        neighborPositionY < 0 || neighborPositionY >= GridSize.y)
                    {
                        continue;
                    }

                    var neighborCell = Cells[neighborPositionX][neighborPositionY];
                    if (neighborCell.IsObstacle) continue;

                    neighborCells[3 * (x + 1) + (y + 1)] = neighborCell;

                }
            }

            return neighborCells;
        }
    }
}