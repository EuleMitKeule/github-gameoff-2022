using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TanksOnAPlain.Unity.Extensions
{
    public static class TransformExtensions
    {
        public static IEnumerable<Transform> GetChildren(this Transform transform) =>
            transform.Cast<Transform>();
    }
}