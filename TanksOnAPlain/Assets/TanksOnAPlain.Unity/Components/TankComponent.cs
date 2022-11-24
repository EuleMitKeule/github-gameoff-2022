using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets;
using TanksOnAPlain.Unity.Components.Health;
using TanksOnAPlain.Unity.Extensions;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components
{
    [RequireComponent(typeof(HealthComponent))]
    public abstract class TankComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public TankAsset TankAsset { get; set; }
        
        [TitleGroup("General")]
        [OdinSerialize]
        [ValueDropdown("ChildObjects")]
        public GameObject TankBody { get; private set; }
        
        [OdinSerialize]
        [ValueDropdown("ChildObjects")]
        public GameObject TankCannon { get; private set; }
        
        [OdinSerialize]
        [ValueDropdown("ChildObjects")]
        public GameObject Graphics { get; private set; }
        
        [UsedImplicitly]
        IEnumerable<GameObject> ChildObjects => gameObject.GetChildren(true);

        HealthComponent HealthComponent { get; set; }

        protected virtual void Awake()
        {
            HealthComponent = GetComponent<HealthComponent>();
            
            HealthComponent.Death += OnDeath;
        }

        protected abstract void OnDeath(object sender, EventArgs e);
    }
}