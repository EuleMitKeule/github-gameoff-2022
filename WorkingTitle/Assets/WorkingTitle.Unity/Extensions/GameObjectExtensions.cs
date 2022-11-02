using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorkingTitle.Unity.Extensions
{
    public static class GameObjectExtensions
    {
        public static IEnumerable<GameObject> GetChildren(this GameObject gameObject) =>
            gameObject.transform.Cast<Transform>().Select(t => t.gameObject);
    }
}