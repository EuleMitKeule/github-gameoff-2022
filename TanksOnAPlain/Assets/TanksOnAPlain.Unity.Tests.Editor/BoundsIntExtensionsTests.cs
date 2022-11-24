using System;
using NUnit.Framework;
using TanksOnAPlain.Unity.Components.Map.Pathfinding;
using TanksOnAPlain.Unity.Extensions;
using UnityEngine;

namespace TanksOnAPlain.Unity.Tests.Editor
{
    public class BoundsIntExtensionsTests
    {
        [Theory]
        public void TestMove(Direction direction)
        {
            var position = new Vector3Int(5, 5);
            var size = new Vector3Int(3, 3, 1);
            var bounds = new BoundsInt(position, size);

            var movedBounds = bounds.MoveBounds(direction);

            var expectedPosition = direction switch
            {
                Direction.None => new Vector3Int(5, 5),
                Direction.Up => new Vector3Int(5, 8),
                Direction.UpLeft => new Vector3Int(2, 8),
                Direction.Left => new Vector3Int(2, 5),
                Direction.DownLeft => new Vector3Int(2, 2),
                Direction.Down => new Vector3Int(5, 2),
                Direction.DownRight => new Vector3Int(8, 2),
                Direction.Right => new Vector3Int(8, 5),
                Direction.UpRight => new Vector3Int(8, 8),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
            var expectedBounds = new BoundsInt(expectedPosition, size);
            
            Assert.AreEqual(expectedBounds.position, movedBounds.position);
            Assert.AreEqual(expectedBounds.size, movedBounds.size);
            Assert.AreEqual(expectedBounds, movedBounds);
        }

        [Test]
        public void TestEncapsulate()
        {
            var position = new Vector3Int(5, 5);
            var size = new Vector3Int(3, 3, 1);
            var bounds = new BoundsInt(position, size);
            
            var otherPosition = new Vector3Int(7, 7);
            var otherSize = new Vector3Int(3, 3, 1);
            var otherBounds = new BoundsInt(otherPosition, otherSize);
            
            var encapsulatedBounds = bounds.Encapsulate(otherBounds);
            
            var expectedPosition = new Vector3Int(5, 5);
            var expectedSize = new Vector3Int(5, 5, 1);
            var expectedBounds = new BoundsInt(expectedPosition, expectedSize);
            
            Assert.AreEqual(expectedBounds.position, encapsulatedBounds.position);
            Assert.AreEqual(expectedBounds.size, encapsulatedBounds.size);
            Assert.AreEqual(expectedBounds, encapsulatedBounds);
        }
    }
}