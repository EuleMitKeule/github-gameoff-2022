using System.Collections.Generic;
using UnityEngine;

namespace TanksOnAPlain.Unity.Extensions
{
    public static class GameObjectExtensions
    {
        public static IEnumerable<GameObject> GetChildren(this GameObject gameObject, bool recursive = false, List<GameObject> children = null)
        {
            children ??= new List<GameObject>();
            
            var newChildren = gameObject.transform.GetChildren();
            
            foreach (var child in newChildren)
            {
                if (recursive) child.gameObject.GetChildren(true, children);
                children.Add(child.gameObject);
            }

            return children;
        }
    }
}