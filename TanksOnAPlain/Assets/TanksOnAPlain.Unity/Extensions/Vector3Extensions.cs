using UnityEngine;

namespace TanksOnAPlain.Unity.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3Int ToCell(this Vector3 position) =>
            new (Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), Mathf.FloorToInt(position.z));
    }
}