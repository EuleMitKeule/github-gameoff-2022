using UnityEngine;

namespace TanksOnAPlain.Unity.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2Int ToCell(this Vector2 position) =>
            new (Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
    }
}