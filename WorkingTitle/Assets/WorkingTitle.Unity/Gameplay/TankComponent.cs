using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Extensions;

namespace WorkingTitle.Unity.Gameplay
{
    public class TankComponent : SerializedMonoBehaviour
    {
        [TitleGroup("General")]
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
    }
}