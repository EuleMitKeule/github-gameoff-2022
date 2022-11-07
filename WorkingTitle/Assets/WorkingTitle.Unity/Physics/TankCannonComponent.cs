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
        [OdinSerialize]
        [ShowIf(nameof(IsAimModeRotational))]
        public float RotationSpeed { get; set; }
        
        TankComponent TankComponent { get; set; }
        
        InputComponent InputComponent { get; set; }
        
        #region Editor
        
        bool IsAimModeRotational => 
            GetComponent<InputComponent>() && GetComponent<InputComponent>().SelectedAimMode == InputComponent.AimMode.Rotational;
        
        #endregion

        void Awake()
        {
            TankComponent = GetComponent<TankComponent>();
            InputComponent = GetComponent<InputComponent>();
        }
        
        void FixedUpdate()
        {
            if (InputComponent.SelectedAimMode == InputComponent.AimMode.Directional)
            {
                var angle = Vector2.SignedAngle(transform.up, InputComponent.InputAimDirection);
                TankComponent.TankCannon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                var angle = TankComponent.TankCannon.transform.rotation.eulerAngles.z + RotationSpeed * InputComponent.InputAimRotation * Time.fixedDeltaTime;
                TankComponent.TankCannon.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}