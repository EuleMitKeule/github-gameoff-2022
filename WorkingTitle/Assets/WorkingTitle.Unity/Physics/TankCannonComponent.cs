using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Input;

namespace WorkingTitle.Unity.Physics
{
    [RequireComponent(typeof(InputComponent))]
    public class TankCannonComponent : SerializedMonoBehaviour
    {
        [TitleGroup("General")]
        [OdinSerialize]
        [ValueDropdown("ChildObjects")]
        protected GameObject TankCannon { get; private set; }
        
        [UsedImplicitly]
        IEnumerable<GameObject> ChildObjects => gameObject.GetChildren();
        
        InputComponent InputComponent { get; set; }

        void Awake()
        {
            InputComponent = GetComponent<InputComponent>();
        }
        
        void Update()
        {
            var direction = InputComponent.InputAimPosition - (Vector2)transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            TankCannon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}