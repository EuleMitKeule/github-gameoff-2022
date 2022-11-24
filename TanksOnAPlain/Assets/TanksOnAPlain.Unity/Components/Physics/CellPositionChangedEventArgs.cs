using System;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Physics
{
    public class CellPositionChangedEventArgs : EventArgs
    {
        public Vector2Int NewCellPosition { get; }
        public Vector2Int OldCellPosition { get; }

        public CellPositionChangedEventArgs(Vector2Int newCellPosition, Vector2Int oldCellPosition)
        {
            NewCellPosition = newCellPosition;
            OldCellPosition = oldCellPosition;
        }
    }
}