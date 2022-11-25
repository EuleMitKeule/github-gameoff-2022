using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Map.Pathfinding
{
    public struct FlowFieldJob : IJob
    {
        BoundsInt bounds;
        Vector2Int targetPosition;
        NativeArray<Vector2Int> obstaclePositions;
        public NativeHashMap<Vector2Int, PathfindingCell> Cells;
        PathfindingCell targetCell;

        public FlowFieldJob(BoundsInt bounds, Vector2Int targetPosition, NativeArray<Vector2Int> obstaclePositions, NativeHashMap<Vector2Int, PathfindingCell> cells)
        {
            this.bounds = bounds;
            this.targetPosition = targetPosition;
            this.obstaclePositions = obstaclePositions;
            targetCell = new PathfindingCell();
            Cells = cells;
        }
        
        public void Execute()
        {
            for (var x = bounds.xMin; x < bounds.xMax + 1; x++)
            {
                for (var y = bounds.yMin; y < bounds.yMax + 1; y++)
                {
                    var position = new Vector2Int(x, y);
                    var isTargetCell = position == targetPosition;
                    var baseCost = isTargetCell ? 0 : 1;
                    var isObstacle = obstaclePositions.Contains(position);
                    var cost = isTargetCell ? 0 : float.PositiveInfinity;

                    var cell = new PathfindingCell(
                        position,
                        (byte)baseCost,
                        isObstacle,
                        cost);
                    
                    Cells.Add(position, cell);
                }
            }

            targetCell = Cells[targetPosition];
            CalcCosts();
            CalcDirections();
        }
        
        void CalcCosts()
        {
            var queue = new Queue<PathfindingCell>();
            queue.Enqueue(targetCell);

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
                        if (queue.Count > 1_000_000)
                        {
                            continue;
                        }
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
                        Cells[neighborPosition] = neighborCell;
                        queue.Enqueue(neighborCell);
                    }
                }
            }
        }

        void CalcDirections()
        {
            for (var x = bounds.xMin; x < bounds.xMax + 1; x++)
            {
                for (var y = bounds.yMin; y < bounds.yMax + 1; y++)
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
                            Cells[position] = cell;
                        }
                    }
                }
            }
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

                    if (neighborPositionX < bounds.xMin || neighborPositionX > bounds.xMax ||
                        neighborPositionY < bounds.yMin || neighborPositionY > bounds.yMax)
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