using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets.Pooling;
using TanksOnAPlain.Unity.Components.Physics;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Pooling
{
    public class PoolComponent : SerializedMonoBehaviour
    {
        // [OdinSerialize]
        // PoolAsset PoolAsset { get; set; }

        [OdinSerialize]
        List<Pool> Pools { get; set; } = new();

        void Awake()
        {
            foreach (var pool in Pools)
            {
                if (pool.PoolObject) continue;
        
                CreatePoolObject(pool);
            }
        }

        public GameObject Allocate(GameObject prefab, Vector3? position = null, Quaternion? rotation = null)
        {
            var pool = Pools.FirstOrDefault(p => p.Prefab == prefab);
            pool ??= CreatePool(prefab);
            
            position ??= Vector3.zero;
            rotation ??= Quaternion.identity;
            
            if (pool.HasLimit && pool.ActiveObjectCount >= pool.ActiveObjectLimit) return null;
            
            var pooledObject = GetOrCreateObject(pool, position.Value, rotation.Value);
            var resettableComponents = pooledObject.GetComponentsInChildren<IResettable>();

            foreach (var resettableComponent in resettableComponents)
            {
                resettableComponent.Reset();
            }
            
            pool.ActiveObjectCount += 1;
            pooledObject.SetActive(true);
            
            return pooledObject;
        }

        GameObject GetOrCreateObject(Pool pool, Vector3 position, Quaternion rotation)
        {
            pool.Objects ??= new();
            var availableObjects = pool.Objects;
            
            if (availableObjects.Count > 0)
            {
                var dequeuedObject = availableObjects.Dequeue();
                dequeuedObject.transform.position = position;
                dequeuedObject.transform.rotation = rotation;

                return dequeuedObject;
            }
            
            var poolObject = pool.PoolObject;
            var pooledObject = Instantiate(pool.Prefab, position, rotation, transform);
            var destroyableComponents = pooledObject.GetComponents<IDestroyable>();
            
            pooledObject.transform.SetParent(poolObject.transform);

            foreach (var destroyableComponent in destroyableComponents)
            {
                destroyableComponent.Destroyed += (sender, e) => OnObjectDestroyed(sender, e, pool.Prefab); 
            }
            
            return pooledObject;
        }
        
        Pool CreatePool(GameObject prefab)
        {
            var pool = new Pool
            {
                Prefab = prefab
            };
            
            CreatePoolObject(pool);
            
            Pools.Add(pool);

            return pool;
        }

        void OnObjectDestroyed(object sender, EventArgs e, GameObject prefab)
        {
            var pool = Pools.First(e => e.Prefab == prefab);
            var component = (Component)sender;
            var poolableObject = component.gameObject;
            
            poolableObject.SetActive(false);
            pool.Objects.Enqueue(poolableObject);
            pool.ActiveObjectCount -= 1;
        }

        void CreatePoolObject(Pool pool)
        {
            var poolObject = new GameObject($"pool_{pool.Prefab.name}");
            poolObject.transform.SetParent(transform);

            pool.PoolObject = poolObject;
        }

        bool HasPool(GameObject prefab) => 
            Pools.FirstOrDefault(e => e.Prefab == prefab) != null;
    }
}