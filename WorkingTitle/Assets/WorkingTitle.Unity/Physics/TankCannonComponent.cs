using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Gameplay;
using WorkingTitle.Unity.Input;

namespace WorkingTitle.Unity.Physics
{
    [RequireComponent(typeof(TankComponent))]
    [RequireComponent(typeof(InputComponent))]
    public class TankCannonComponent : SerializedMonoBehaviour
    {
        TankComponent TankComponent { get; set; }
        
        InputComponent InputComponent { get; set; }

        void Awake()
        {
            TankComponent = GetComponent<TankComponent>();
            InputComponent = GetComponent<InputComponent>();
        }
        
        void Update()
        {
            var direction = InputComponent.InputAimPosition - (Vector2)transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            TankComponent.TankCannon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}