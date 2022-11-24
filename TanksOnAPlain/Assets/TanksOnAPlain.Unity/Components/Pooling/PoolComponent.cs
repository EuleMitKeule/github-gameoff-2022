using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TanksOnAPlain.Unity.Components.Physics;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Pooling
{
    public class PoolComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        Dictionary<GameObject, Queue<GameObject>> AvailableObjectsPerPrefab { get; } = new();
        Dictionary<GameObject, GameObject> PrefabToPool { get; } = new();

        public GameObject Allocate(GameObject prefab, Vector3? position = null, Quaternion? rotation = null)
        {
            position ??= Vector3.zero;
            rotation ??= Quaternion.identity;
            
            if (!AvailableObjectsPerPrefab.ContainsKey(prefab))
            {
                CreatePool(prefab);
            }

            var pooledObject = GetOrCreateObject(prefab, position.Value, rotation.Value);
            var resettableComponents = pooledObject.GetComponentsInChildren<IResettable>();

            foreach (var resettableComponent in resettableComponents)
            {
                resettableComponent.Reset();
            }
            
            pooledObject.SetActive(true);
            return pooledObject;
        }

        GameObject GetOrCreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var availableObjects = AvailableObjectsPerPrefab[prefab];
            
            if (availableObjects.Count > 0)
            {
                var dequeuedObject = availableObjects.Dequeue();
                dequeuedObject.transform.position = position;
                dequeuedObject.transform.rotation = rotation;
                
                return dequeuedObject;
            }
            
            var poolObject = PrefabToPool[prefab];
            var pooledObject = Instantiate(prefab, position, rotation, transform);
            var destroyableComponents = pooledObject.GetComponents<IDestroyable>();
            
            pooledObject.transform.SetParent(poolObject.transform);

            foreach (var destroyableComponent in destroyableComponents)
            {
                destroyableComponent.Destroyed += (sender, e) => OnObjectDestroyed(sender, e, prefab); 
            }
            
            return pooledObject;
        }
        
        void CreatePool(GameObject prefab)
        {
            AvailableObjectsPerPrefab[prefab] = new Queue<GameObject>();
            
            var poolObject = new GameObject($"pool_{prefab.name}");
            poolObject.transform.SetParent(transform);
            
            PrefabToPool[prefab] = poolObject;
        }

        void OnObjectDestroyed(object sender, EventArgs e, GameObject prefab)
        {
            var component = (Component)sender;
            var poolableObject = component.gameObject;
            
            poolableObject.SetActive(false);
            AvailableObjectsPerPrefab[prefab].Enqueue(poolableObject);
        }
    }
}