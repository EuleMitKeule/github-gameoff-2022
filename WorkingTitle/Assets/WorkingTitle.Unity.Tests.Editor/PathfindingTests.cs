using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using WorkingTitle.Lib.Pathfinding;

namespace WorkingTitle.Unity.Tests.Editor
{
    public class PathfindingTests
    {
        [Test]
        public void TestCalcDirections()
        {
            var targetPosition = new Vector2Int(2, 4);
            var obstaclePositions = new List<Vector2Int>();
            var gridSize = new Vector2Int(5, 5);
            
            var directions = Pathfinding.CalcDirections(targetPosition, obstaclePositions, gridSize);
            
            var expectedDirections = new [,]
            {
                {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.UpRight, PathfindingDirection.Right, PathfindingDirection.Right},
                {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.UpRight, PathfindingDirection.Right},
                {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.None},
                {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.UpLeft, PathfindingDirection.Left},
                {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.UpLeft, PathfindingDirection.UpLeft, PathfindingDirection.Left}
            };
            
            CollectionAssert.AreEqual(expectedDirections, directions);
        }
        
        [Test]
        public void TestCalcDirectionsWithWall()
        {
            var targetPosition = new Vector2Int(2, 4);
            var obstaclePositions = new List<Vector2Int>
            {
                new (3, 2)
            };
            var gridSize = new Vector2Int(5, 5);
            
            var directions = Pathfinding.CalcDirections(targetPosition, obstaclePositions, gridSize);
            
            var expectedDirections = new [,]
            {
                {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.UpRight, PathfindingDirection.Right, PathfindingDirection.Right},
                {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.UpRight, PathfindingDirection.Right},
                {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.None},
                {PathfindingDirection.Up, PathfindingDirection.UpLeft, PathfindingDirection.None, PathfindingDirection.UpLeft, PathfindingDirection.Left},
                {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.UpLeft, PathfindingDirection.UpLeft, PathfindingDirection.Left}
            };
            
            CollectionAssert.AreEqual(expectedDirections, directions);
        }
        
        [Test]
        public void TestCalcDirectionsWithWalls()
        {
            var targetPosition = new Vector2Int(2, 4);
            var obstaclePositions = new List<Vector2Int>
            {
                new (0, 1),
                new (1, 1),
                new (3, 2),
                new (3, 3),
                new (4, 3)
            };
            var gridSize = new Vector2Int(5, 5);
            
            var directions = Pathfinding.CalcDirections(targetPosition, obstaclePositions, gridSize);
            
            var expectedDirections = new [,]
            {
                {PathfindingDirection.Right, PathfindingDirection.None, PathfindingDirection.UpRight, PathfindingDirection.Right, PathfindingDirection.Right},
                {PathfindingDirection.UpRight, PathfindingDirection.None, PathfindingDirection.Up, PathfindingDirection.UpRight, PathfindingDirection.Right},
                {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.None},
                {PathfindingDirection.Up, PathfindingDirection.UpLeft, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.Left},
                {PathfindingDirection.UpLeft, PathfindingDirection.Left, PathfindingDirection.DownLeft, PathfindingDirection.None, PathfindingDirection.Left}
            };
            
            CollectionAssert.AreEqual(expectedDirections, directions);
        }
        
        [Test]
        public void TestCalcDirectionsWithImpossiblePath()
        {
            var targetPosition = new Vector2Int(2, 4);
            var obstaclePositions = new List<Vector2Int>
            {
                new (0, 2),
                new (1, 2),
                new (2, 2),
                new (3, 2),
                new (4, 2)
            };
            var gridSize = new Vector2Int(5, 5);
            
            var directions = Pathfinding.CalcDirections(targetPosition, obstaclePositions, gridSize);
            
            var expectedDirections = new [,]
            {
                {PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.Right, PathfindingDirection.Right},
                {PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.UpRight, PathfindingDirection.Right},
                {PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.Up, PathfindingDirection.None},
                {PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.UpLeft, PathfindingDirection.Left},
                {PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.UpLeft, PathfindingDirection.Left}
            };
            
            CollectionAssert.AreEqual(expectedDirections, directions);
        }
    }
}