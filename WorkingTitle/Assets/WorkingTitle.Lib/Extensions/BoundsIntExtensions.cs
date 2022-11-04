using UnityEngine;
using WorkingTitle.Lib.Pathfinding;

namespace WorkingTitle.Lib.Extensions
{
    public static class BoundsIntExtensions
    {
        public static BoundsInt MoveBounds(this BoundsInt bounds, Direction direction)
        {
            var offset = direction.ToVector3Int();
            var position = bounds.position + offset * bounds.size;
            return new BoundsInt(position, bounds.size);
        }
        
        public static BoundsInt Encapsulate(this BoundsInt bounds, BoundsInt otherBounds)
        {
            var encapsulatedBounds = new BoundsInt
            {
                xMin = Mathf.Min(bounds.xMin, otherBounds.xMin),
                xMax = Mathf.Max(bounds.xMax, otherBounds.xMax),
                yMin = Mathf.Min(bounds.yMin, otherBounds.yMin),
                yMax = Mathf.Max(bounds.yMax, otherBounds.yMax),
                zMin = Mathf.Min(bounds.zMin, otherBounds.zMin),
                zMax = Mathf.Max(bounds.zMax, otherBounds.zMax)
            };

            return encapsulatedBounds;
        }

        public static BoundsInt ToPositive(this BoundsInt bounds)
        {
            var positiveBounds = new BoundsInt
            {
                xMin = 0,
                xMax = bounds.size.x,
                yMin = 0,
                yMax = bounds.size.y,
                zMin = 0,
                zMax = bounds.size.z
            };

            return positiveBounds;
        }
    }
}