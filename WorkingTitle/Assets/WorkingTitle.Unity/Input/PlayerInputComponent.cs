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
        
        [OdinSerialize]
        [Required]
        [LabelText("Primary Attack Action")]
        [EnableIf("InputActionMap")]
        [ValueDropdown("InputActionNames")]
        string InputActionPrimaryAttackName { get; set; }

        Camera Camera { get; set; }
        
        InputActionMap InputActionMap => 
            InputActionMapName is not null ? InputActionAsset.FindActionMap(InputActionMapName) : null;
        InputAction InputActionMovement =>
            InputActionMovementName is not null ? InputActionMap.FindAction(InputActionMovementName) : null;
        InputAction InputActionRotation => 
            InputActionMap.FindAction(InputActionRotationName);
        InputAction InputActionPrimaryAttack =>
            InputActionPrimaryAttackName is not null ? InputActionMap.FindAction(InputActionPrimaryAttackName) : null;

        [UsedImplicitly]
        IEnumerable<string> InputActionMapNames => InputActionAsset ? InputActionAsset.actionMaps.Select(e => e.name) : null;
        
        [UsedImplicitly]
        IEnumerable<string> InputActionNames => InputActionMap?.actions.Select(e => e.name);
        
        void Awake()
        {
            InputActionMovement.Enable();
            InputActionRotation.Enable();
            InputActionPrimaryAttack.Enable();
            InputActionMovement.started += OnMovementStarted;
            InputActionRotation.started += OnRotationStarted;
            InputActionPrimaryAttack.started += OnPrimaryAttackStarted;
            InputActionMovement.canceled += OnMovementCanceled;
            InputActionRotation.canceled += OnRotationCanceled;
            InputActionPrimaryAttack.canceled += OnPrimaryAttackCanceled;

            Camera = Camera.main;
        }

        void Update()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var mousePositionWorld = (Vector2)Camera.ScreenToWorldPoint(mousePosition);
            InputAimPosition = mousePositionWorld;
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

        void OnPrimaryAttackStarted(InputAction.CallbackContext context)
        {
            InputPrimaryAttack = true;
        }

        void OnRotationCanceled(InputAction.CallbackContext context)
        {
            InputRotation = 0;
        }

        void OnMovementCanceled(InputAction.CallbackContext context)
        {
            InputMovement = 0;
        }

        void OnPrimaryAttackCanceled(InputAction.CallbackContext context)
        {
            InputPrimaryAttack = false;
        }
    }
}