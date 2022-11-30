using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Pooling
{
    [Serializable]
    public class Pool
    {
        [OdinSerialize]
        public GameObject Prefab { get; set; }

        [OdinSerialize]
        public GameObject PoolObject { get; set; }
        
        [OdinSerialize]
        public int ActiveObjectLimit { get; set; }
        
        [OdinSerialize]
        public bool HasLimit { get; set; }

        [ShowInInspector]
        [ShowIf(nameof(IsPlaying))]
        [ReadOnly]
        public int ActiveObjectCount { get; set; }
        
        [ShowInInspector]
        [ShowIf(nameof(IsPlaying))]
        [ReadOnly]
        public Queue<GameObject> Objects { get; set; } = new();
        
        bool IsPlaying => Application.isPlaying;
    }
}