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
    public class TankComponent : SerializedMonoBehaviour
    {
        [TitleGroup("General")]
        [OdinSerialize]
        public TankAsset TankAsset { get; set; }
        
        [OdinSerialize]
        [ValueDropdown("ChildObjects")]
        public GameObject TankBody { get; private set; }
        
        [OdinSerialize]
        [ValueDropdown("ChildObjects")]
        public GameObject TankCannon { get; private set; }
        
        [UsedImplicitly]
        IEnumerable<GameObject> ChildObjects => gameObject.GetChildren();
        
        [TitleGroup("Physics")]
        [OdinSerialize]
        public LayerMask WallMask { get; set; }
        
        HealthComponent HealthComponent { get; set; }

        void Awake()
        {
            HealthComponent = GetComponent<HealthComponent>();
            
            HealthComponent.Death += OnDeath;
        }

        void OnDeath(object sender, EventArgs e)
        {
            Destroy(gameObject);
        }
    }
}