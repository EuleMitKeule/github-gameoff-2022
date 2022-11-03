using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorkingTitle.Lib.Extensions;

namespace WorkingTitle.Lib.Pathfinding
{
    public class FlowField
    {
        public PathfindingCell TargetCell { get; }
        public PathfindingCell[,] Cells { get; }
        
        public Vector2Int GridSize { get; }

        public bool IsCostsCalculated { get; private set; }
        public bool IsDirectionsCalculated { get; private set; }
        
        public FlowField(
            Vector2Int targetPosition,
            List<Vector2Int> obstaclePositions,
            Vector2Int gridSize)
        {
            Cells = new PathfindingCell[gridSize.x, gridSize.y];
            
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
                    Cells[x, y] = cell;
                }
            }
            
            TargetCell = Cells[targetPosition.x, targetPosition.y];
            GridSize = gridSize;
        }
        
        public void CalcCosts()
        {
            var queue = new Queue<PathfindingCell>();
            queue.Enqueue(TargetCell);

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                var neighborCells = GetNeighborCells(cell.Position, PathfindingDirectionExtensions.Cardinal);
                
                foreach (var neighborCell in neighborCells)
                {
                    var neighborCost = (ushort)(neighborCell.BaseCost + cell.Cost);
                    
                    if (neighborCost >= neighborCell.Cost) continue;
                    
                    neighborCell.Cost = neighborCost;
                    queue.Enqueue(neighborCell);
                }
            }
            
            IsCostsCalculated = true;
        }

        public void CalcDirections()
        {
            foreach (var cell in Cells)
            {
                if (cell.IsObstacle) continue;
                
                var neighborCells = GetNeighborCells(cell.Position, PathfindingDirectionExtensions.CardinalAndInterCardinal);
                var bestCost = cell.Cost;
                
                foreach (var neighbor in neighborCells)
                {
                    if (neighbor.Cost >= bestCost) continue;
                    
                    bestCost = neighbor.Cost;
                    var direction = (Vector2)(neighbor.Position - cell.Position);
                    cell.Direction = direction.normalized;
                }
            }
            
            IsDirectionsCalculated = true;
        }

        public PathfindingDirection[,] GetDirections()
        {
            if (!IsCostsCalculated)
            {
                throw new InvalidOperationException("Costs must be calculated before directions can be retrieved.");   
            }
            
            if (!IsDirectionsCalculated)
            {
                throw new InvalidOperationException("Directions must be calculated before directions can be retrieved.");
            }
            
            var directions = new PathfindingDirection[Cells.GetLength(0), Cells.GetLength(1)];
            
            for (var x = 0; x < GridSize.x; x++)
            {
                for (var y = 0; y < GridSize.y; y++)
                {
                    var cell = Cells[x, y];
                    directions[x, y] = cell.Direction.ToPathfindingDirection();
                }
            }
            
            return directions;
        }
        
        List<PathfindingCell> GetNeighborCells(Vector2Int position, IEnumerable<PathfindingDirection> directions)
        {
            var neighborCells = new List<PathfindingCell>();

            foreach (var direction in directions)
            {
                var neighborPosition = position + direction.ToVector2Int();

                if (neighborPosition.x < 0 || neighborPosition.x >= Cells.GetLength(0) ||
                    neighborPosition.y < 0 || neighborPosition.y >= Cells.GetLength(1))
                {
                    continue;
                }
                
                var neighborCell = Cells[neighborPosition.x, neighborPosition.y];
                
                if (neighborCell.IsObstacle)
                {
                    continue;
                }
                
                neighborCells.Add(neighborCell);
            }
            
            return neighborCells;
        }
    }
}