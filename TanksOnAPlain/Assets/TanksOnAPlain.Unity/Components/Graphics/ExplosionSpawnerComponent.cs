using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components.Health;
using TanksOnAPlain.Unity.Components.Pooling;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

namespace TanksOnAPlain.Unity.Components.Graphics
{
    public class ExplosionSpawnerComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        GameObject ExplosionPrefab { get; set; }
        
        PoolComponent PoolComponent { get; set; }
        HealthComponent HealthComponent { get; set; }

        void Awake()
        {
            PoolComponent = FindObjectOfType<PoolComponent>();
            HealthComponent = GetComponent<HealthComponent>();

            HealthComponent.Death += OnDeath;
        }

        void OnDeath(object sender, EventArgs e)
        {
            PoolComponent.Allocate(ExplosionPrefab, transform.position);
        }
    }
}