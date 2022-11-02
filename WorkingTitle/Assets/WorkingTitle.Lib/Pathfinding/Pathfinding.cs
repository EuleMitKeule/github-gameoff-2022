using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorkingTitle.Lib.Pathfinding
{
    public static class Pathfinding
    {
        // TODO: fix walking through walls diagonally
        public static PathfindingDirection[,] CalcDirections(
            Vector2Int targetPosition,
            IEnumerable<Vector2Int> obstaclePositions,
            Vector2Int gridSize)
        {
            if (obstaclePositions is null)
            {
                throw new ArgumentNullException(nameof(obstaclePositions));
            }
            
            if (0 > targetPosition.x || targetPosition.x >= gridSize.x ||
                0 > targetPosition.y || targetPosition.y >= gridSize.y)
            {
                throw new ArgumentException(
                    $"'{nameof(targetPosition)}' ({targetPosition}) must be between (0, 0) and {gridSize}",
                    nameof(targetPosition));
            }

            if (gridSize.x <= 0 || gridSize.y <= 0)
            {
                throw new ArgumentException(
                    $"'{nameof(gridSize)}' ({gridSize}) must be greater than (0, 0)",
                    nameof(gridSize));
            }
            
            if (obstaclePositions.Any(e =>
                0 > e.x || e.x >= gridSize.x ||
                0 > e.y || e.y >= gridSize.y))
            {
                throw new ArgumentException(
                    $"Items in '{nameof(obstaclePositions)}' must be between (0, 0) and {gridSize}",
                    nameof(obstaclePositions));
            }
            
            var directions = new PathfindingDirection[gridSize.x, gridSize.y];
            var costs = new float?[gridSize.x, gridSize.y];
            
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(targetPosition);
            
            directions[targetPosition.x, targetPosition.y] = PathfindingDirection.None;
            
            while (queue.Count > 0)
            {
                var position = queue.Dequeue();
                var neighbors = CalcNeighbors(position)
                    .Where(e =>
                        0 <= e.position.x && e.position.x < gridSize.x &&
                        0 <= e.position.y && e.position.y < gridSize.y &&
                        !obstaclePositions.Contains(e.position));

                var bestDirection = PathfindingDirection.None;
                var bestCost = new float?();
                
                foreach (var (neighborPosition, neighborDirection) in neighbors)
                {
                    var cost = costs[neighborPosition.x, neighborPosition.y] + neighborDirection.ToCost();
                    if (!cost.HasValue)
                    {
                        queue.Enqueue(neighborPosition);
                        continue;
                    }

                    if (cost >= bestCost) continue;
                    bestCost = cost;
                    bestDirection = neighborDirection;
                }
                
                if (!bestCost.HasValue) costs[position.x, position.y] = 0;
                else costs[position.x, position.y] = bestCost.Value;
                directions[position.x, position.y] = bestDirection;
            }

            return directions;
        }
        
        static IEnumerable<(Vector2Int position, PathfindingDirection direction)> CalcNeighbors(Vector2Int position) =>
            PathfindingDirectionExtensions.All
                .Select(direction => (position + direction.ToVector2Int(), direction))
                .ToList();
    }
}