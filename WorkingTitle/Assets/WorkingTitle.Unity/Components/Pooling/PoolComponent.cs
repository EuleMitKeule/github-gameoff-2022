using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WorkingTitle.Unity.Components.Pooling
{
    public class PoolComponent : SerializedMonoBehaviour
    {
        Dictionary<GameObject, Queue<Component>> AvailableObjectsPerPrefab { get; set; } = new();
        
        Dictionary<GameObject, GameObject> PrefabToPool { get; set; } = new();

        public T Allocate<T>(GameObject prefab) where T : Component, IPoolable
        {
            if (!AvailableObjectsPerPrefab.ContainsKey(prefab))
            {
                AvailableObjectsPerPrefab[prefab] = new Queue<Component>();

                var poolObject = new GameObject($"pool_{prefab.name}");
                poolObject.transform.SetParent(transform);
                
                PrefabToPool[prefab] = poolObject;
            }

            var pooledComponent = GetOrCreateObject<T>(prefab);

            return pooledComponent;
        }

        T GetOrCreateObject<T>(GameObject prefab) where T : Component, IPoolable
        {
            var availableObjects = AvailableObjectsPerPrefab[prefab];
            if (availableObjects.Count > 0)
            {
                var dequeuedComponent = (T)availableObjects.Dequeue();
                dequeuedComponent.gameObject.SetActive(true);
                dequeuedComponent.Reset();
                
                return dequeuedComponent;
            }
            
            var pooledObject = Instantiate(prefab, transform);
            var poolObject = PrefabToPool[prefab];
            pooledObject.transform.SetParent(poolObject.transform);

            var pooledComponent = pooledObject.GetComponent<T>();
            pooledComponent.Destroyed += (sender, e) => OnObjectDestroyed(sender, e, prefab);
            
            return pooledComponent;
        }

        void OnObjectDestroyed(object sender, EventArgs e, GameObject prefab)
        {
            var pooledComponent = (Component)sender;
            
            pooledComponent.gameObject.SetActive(false);
            
            AvailableObjectsPerPrefab[prefab].Enqueue(pooledComponent);
        }
    }
}