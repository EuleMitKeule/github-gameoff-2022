using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WorkingTitle.Unity.Input
{
    public class PlayerInputComponent : InputComponent
    {
        [TitleGroup("Input System")]
        [OdinSerialize]
        [Required]
        InputActionAsset InputActionAsset { get; set; }

        [OdinSerialize]
        [Required]
        [LabelText("Input Action Map")]
        [EnableIf("InputActionAsset")]
        [ValueDropdown("InputActionMapNames")]
        string InputActionMapName { get; set; }
        
        [OdinSerialize]
        [Required]
        [LabelText("Movement Action")]
        [EnableIf("InputActionMap")]
        [ValueDropdown("InputActionNames")]
        string InputActionMovementName { get; set; }
        
        [OdinSerialize]
        [Required]
        [LabelText("Rotation Action")]
        [EnableIf("InputActionMap")]
        [ValueDropdown("InputActionNames")]
        string InputActionRotationName { get; set; }

        Camera Camera { get; set; }
        
        InputActionMap InputActionMap => 
            InputActionMapName is not null ? InputActionAsset.FindActionMap(InputActionMapName) : null;
        InputAction InputActionMovement =>
            InputActionMovementName is not null ? InputActionMap.FindAction(InputActionMovementName) : null;
        InputAction InputActionRotation => 
            InputActionMap.FindAction(InputActionRotationName);

        [UsedImplicitly]
        IEnumerable<string> InputActionMapNames => InputActionAsset ? InputActionAsset.actionMaps.Select(e => e.name) : null;
        
        [UsedImplicitly]
        IEnumerable<string> InputActionNames => InputActionMap?.actions.Select(e => e.name);
        
        void Awake()
        {
            InputActionMovement.Enable();
            InputActionRotation.Enable();
            InputActionMovement.started += OnMovementStarted;
            InputActionRotation.started += OnRotationStarted;
            InputActionMovement.canceled += OnMovementCanceled;
            InputActionRotation.canceled += OnRotationCanceled;

            Camera = Camera.main;
        }

        void Update()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var mousePositionWorld = (Vector2)Camera.ScreenToWorldPoint(mousePosition);
            InputAimPosition = mousePositionWorld;
        }

        void OnRotationCanceled(InputAction.CallbackContext context)
        {
            InputRotation = 0;
        }

        void OnMovementCanceled(InputAction.CallbackContext obj)
        {
            InputMovement = 0;
        }

        void OnRotationStarted(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();
            InputRotation = value;
        }

        void OnMovementStarted(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();
            InputMovement = value;
        }
    }
}