using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using TanksOnAPlain.Unity.Components.Map.Pathfinding;
using UnityEngine;

namespace TanksOnAPlain.Unity.Tests.Editor
{
    public class FlowFieldTests
    {
        // [Test]
        // public void TestCalcDirections()
        // {
        //     var targetPosition = new Vector2Int(2, 4);
        //     var obstaclePositions = new List<Vector2Int>();
        //     var gridSize = new Vector2Int(5, 5);
        //     
        //     var flowField = new FlowField(targetPosition, obstaclePositions, gridSize);
        //     flowField.CalcCosts();
        //     flowField.CalcDirections();
        //     
        //     var expectedDirections = new[,]
        //     {
        //         { Direction.UpRight, Direction.UpRight, Direction.UpRight, Direction.UpRight, Direction.Right },
        //         { Direction.UpRight, Direction.UpRight, Direction.UpRight, Direction.UpRight, Direction.Right },
        //         { Direction.Up,     Direction.Up,       Direction.Up,      Direction.Up,     Direction.None },
        //         { Direction.UpLeft, Direction.UpLeft, Direction.UpLeft, Direction.UpLeft, Direction.Left },
        //         { Direction.UpLeft, Direction.UpLeft, Direction.UpLeft, Direction.UpLeft, Direction.Left }
        //     };
        //
        //     var directions = flowField.GetDirections();
        //     
        //     CollectionAssert.AreEqual(expectedDirections, directions);
        // }
        //
        // [Test]
        // public void TestCalcDirectionsWithWall()
        // {
        //     var targetPosition = new Vector2Int(2, 4);
        //     var obstaclePositions = new List<Vector2Int>
        //     {
        //         new (3, 2)
        //     };
        //     var gridSize = new Vector2Int(5, 5);
        //     
        //     var directions = FlowField.CalcDirections(targetPosition, obstaclePositions, gridSize);
        //     
        //     var expectedDirections = new [,]
        //     {
        //         {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.UpRight, PathfindingDirection.Right, PathfindingDirection.Right},
        //         {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.UpRight, PathfindingDirection.Right},
        //         {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.None},
        //         {PathfindingDirection.Up, PathfindingDirection.UpLeft, PathfindingDirection.None, PathfindingDirection.UpLeft, PathfindingDirection.Left},
        //         {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.UpLeft, PathfindingDirection.UpLeft, PathfindingDirection.Left}
        //     };
        //     
        //     CollectionAssert.AreEqual(expectedDirections, directions);
        // }
        //
        // [Test]
        // public void TestCalcDirectionsWithWalls()
        // {
        //     var targetPosition = new Vector2Int(2, 4);
        //     var obstaclePositions = new List<Vector2Int>
        //     {
        //         new (0, 1),
        //         new (1, 1),
        //         new (3, 2),
        //         new (3, 3),
        //         new (4, 3)
        //     };
        //     var gridSize = new Vector2Int(5, 5);
        //     
        //     var directions = FlowField.CalcDirections(targetPosition, obstaclePositions, gridSize);
        //     
        //     var expectedDirections = new [,]
        //     {
        //         {PathfindingDirection.Right, PathfindingDirection.None, PathfindingDirection.UpRight, PathfindingDirection.Right, PathfindingDirection.Right},
        //         {PathfindingDirection.UpRight, PathfindingDirection.None, PathfindingDirection.Up, PathfindingDirection.UpRight, PathfindingDirection.Right},
        //         {PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.Up, PathfindingDirection.None},
        //         {PathfindingDirection.Up, PathfindingDirection.UpLeft, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.Left},
        //         {PathfindingDirection.UpLeft, PathfindingDirection.Left, PathfindingDirection.DownLeft, PathfindingDirection.None, PathfindingDirection.Left}
        //     };
        //     
        //     CollectionAssert.AreEqual(expectedDirections, directions);
        // }
        //
        // [Test]
        // public void TestCalcDirectionsWithImpossiblePath()
        // {
        //     var targetPosition = new Vector2Int(2, 4);
        //     var obstaclePositions = new List<Vector2Int>
        //     {
        //         new (0, 2),
        //         new (1, 2),
        //         new (2, 2),
        //         new (3, 2),
        //         new (4, 2)
        //     };
        //     var gridSize = new Vector2Int(5, 5);
        //     
        //     var directions = FlowField.CalcDirections(targetPosition, obstaclePositions, gridSize);
        //     
        //     var expectedDirections = new [,]
        //     {
        //         {PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.Right, PathfindingDirection.Right},
        //         {PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.UpRight, PathfindingDirection.Right},
        //         {PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.Up, PathfindingDirection.None},
        //         {PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.UpLeft, PathfindingDirection.Left},
        //         {PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.None, PathfindingDirection.UpLeft, PathfindingDirection.Left}
        //     };
        //     
        //     CollectionAssert.AreEqual(expectedDirections, directions);
        // }
        //
        [Theory]
        [TestCase(5, 5)]
        [TestCase(15, 15)]
        [TestCase(25, 25)]
        [TestCase(35, 35)]
        [TestCase(100, 100)]
        [TestCase(500, 500)]
        public void TestPathfindingPerformance(int gridX, int gridY)
        {
            var targetPosition = new Vector2Int(0, 0);
            var obstaclePositions = new List<Vector2Int>();
            var bounds = new BoundsInt(Vector3Int.zero, new Vector3Int(gridX, gridY, 1));
        
            var sw = Stopwatch.StartNew();
            
            var flowField = new FlowField(targetPosition, obstaclePositions, bounds);
            
            sw.Stop();
            var initializationTime = sw.ElapsedMilliseconds;
            
            sw.Restart();

            flowField.CalcCosts();
            
            sw.Stop();
            var calcCostsTime = sw.ElapsedMilliseconds;
            
            sw.Restart();
            
            flowField.CalcDirections();
            
            sw.Stop();
            var calcDirectionsTime = sw.ElapsedMilliseconds;

            TestContext.Out.WriteLine($"Grid Size: {bounds}");
            TestContext.Out.WriteLine($"Initialization: {initializationTime} ms");
            TestContext.Out.WriteLine($"CalcCosts: {calcCostsTime} ms");
            TestContext.Out.WriteLine($"CalcDirections: {calcDirectionsTime} ms");
            TestContext.Out.WriteLine($"Total: {initializationTime + calcCostsTime + calcDirectionsTime} ms");
        }
    }
}