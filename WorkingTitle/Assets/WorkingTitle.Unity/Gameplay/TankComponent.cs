using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Extensions;

namespace WorkingTitle.Unity.Gameplay
{
    [RequireComponent(typeof(HealthComponent))]
    public abstract class TankComponent : SerializedMonoBehaviour
    {
        [TitleGroup("General")]
        [OdinSerialize]
        [ValueDropdown("ChildObjects")]
        public GameObject TankBody { get; private set; }
        
        [OdinSerialize]
        [ValueDropdown("ChildObjects")]
        public GameObject TankCannon { get; private set; }
        
        [UsedImplicitly]
        IEnumerable<GameObject> ChildObjects => gameObject.GetChildren(true);
        
        [TitleGroup("Physics")]
        [OdinSerialize]
        public LayerMask WallMask { get; set; }
        
        HealthComponent HealthComponent { get; set; }

        void Awake()
        {
            HealthComponent = GetComponent<HealthComponent>();
            
            HealthComponent.Death += OnDeath;
        }

        protected abstract void OnDeath(object sender, EventArgs e);
    }
}