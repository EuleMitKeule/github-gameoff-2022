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

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                var neighborCells =
                    GetNeighborCells(cell.Position, PathfindingDirectionExtensions.CardinalAndInterCardinal);

                foreach (var neighborDirection in PathfindingDirectionExtensions.CardinalAndInterCardinal)
                {
                    var relativePosition = neighborDirection.ToVector2Int();
                    var neighborCell = neighborCells[relativePosition.x + 1][relativePosition.y + 1];
                    var isInterCardinal = relativePosition.magnitude > 1;

                    if (neighborCell is null || neighborCell.IsObstacle) continue;
                    
                    if (isInterCardinal &&
                        ((neighborCells[relativePosition.x + 1][1]?.IsObstacle ?? true) || 
                        (neighborCells[1][relativePosition.y + 1]?.IsObstacle ?? true)))
                    {
                        continue;
                    }
                    
                    var neighborCost = (ushort)(neighborCell.BaseCost * (isInterCardinal ? Mathf.Sqrt(2) : 1) + cell.Cost);
                    
                    if (neighborCost >= neighborCell.Cost) continue;
                    
                    neighborCell.Cost = neighborCost;
                    queue.Enqueue(neighborCell);
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
                
                    var neighborCells = GetNeighborCells(cell.Position, PathfindingDirectionExtensions.CardinalAndInterCardinal);
                    var bestCost = cell.Cost;
                
                    foreach (var direction in PathfindingDirectionExtensions.CardinalAndInterCardinal)
                    {
                        var relativePosition = direction.ToVector2Int();
                        var neighborCell = neighborCells[relativePosition.x + 1][relativePosition.y + 1];
                        var isInterCardinal = relativePosition.magnitude > 1;
                    
                        if (neighborCell is null || neighborCell.Cost >= bestCost) continue;
                    
                        if (isInterCardinal &&
                            ((neighborCells[relativePosition.x + 1][1]?.IsObstacle ?? true) || 
                             (neighborCells[1][relativePosition.y + 1]?.IsObstacle ?? true)))
                        {
                            continue;
                        }
                    
                        if (neighborCell.Cost >= bestCost) continue;
                    
                        bestCost = neighborCell.Cost;
                        cell.Direction = direction;
                    }
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
            
            var directions = new PathfindingDirection[GridSize.x, GridSize.y];
            
            for (var x = 0; x < GridSize.x; x++)
            {
                for (var y = 0; y < GridSize.y; y++)
                {
                    var cell = Cells[x][y];
                    directions[x, y] = cell.Direction;
                }
            }
            
            return directions;
        }

        PathfindingCell GetNeighborCell(Vector2Int position, PathfindingDirection direction)
        {
            var neighborPosition = position + direction.ToVector2Int();

            if (neighborPosition.x < 0 || neighborPosition.x >= GridSize.x ||
                neighborPosition.y < 0 || neighborPosition.y >= GridSize.y)
            {
                return null;
            }
                
            var neighborCell = Cells[neighborPosition.x][neighborPosition.y];
                
            return neighborCell.IsObstacle ? null : neighborCell;
        }
        
        PathfindingCell[][] GetNeighborCells(Vector2Int position, IEnumerable<PathfindingDirection> directions)
        {
            var neighborCells = new PathfindingCell[3][];
            for (var i = 0; i < 3; i++)
            {
                neighborCells[i] = new PathfindingCell[3];
            }

            foreach (var direction in directions)
            {
                var relativePosition = direction.ToVector2Int();
                var neighborCell = GetNeighborCell(position, direction);
                neighborCells[relativePosition.x + 1][relativePosition.y + 1] = neighborCell;
            }

            return neighborCells;
        }
    }
}