using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Map.Pathfinding
{
    public struct PathfindingJob : IJob
    {
        public Vector2Int TargetCellPosition { get; set; }
        public NativeArray<Vector2Int> ObstaclePositions { get; set; }
        public BoundsInt MapBounds { get; set; }

        public NativeArray<FlowField> NativeFlowField;
        
        public void Execute()
        {
            var flowField = new FlowField(TargetCellPosition, ObstaclePositions.ToList(), MapBounds);
            NativeFlowField[0] = flowField;
        }
    }
}