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

        [OdinSerialize]
        [Required]
        [LabelText("Boost Action")]
        [EnableIf("InputActionMap")]
        [ValueDropdown("InputActionNames")]
        string InputActionBoostName { get; set; }

        Camera Camera { get; set; }
        
        InputActionMap InputActionMap { get; set; }
        InputAction InputActionMovement { get; set; }
        InputAction InputActionRotation { get; set; }
        InputAction InputActionBoost { get; set; }
        InputAction InputActionPrimaryAttack { get; set; }

        # region Editor
        
        [UsedImplicitly]
        IEnumerable<string> InputActionMapNames => 
            InputActionAsset ? InputActionAsset.actionMaps.Select(e => e.name) : null;
        
        [UsedImplicitly]
        IEnumerable<string> InputActionNames => 
            InputActionMap?.actions.Select(e => e.name);
        
        # endregion
        
        void Awake()
        {
            InitializeActions();
            
            InputActionMovement.started += OnMovementStarted;
            InputActionRotation.started += OnRotationStarted;
            InputActionPrimaryAttack.started += OnPrimaryAttackStarted;
            InputActionBoost.started += OnBoostStarted;
            InputActionMovement.canceled += OnMovementCanceled;
            InputActionRotation.canceled += OnRotationCanceled;
            InputActionPrimaryAttack.canceled += OnPrimaryAttackCanceled;
            InputActionBoost.canceled += OnBoostCanceled;

            Camera = Camera.main;
        }

        void InitializeActions()
        {
            InputActionMap = InputActionAsset.FindActionMap(InputActionMapName);
            
            InputActionMovement = InputActionMap.FindAction(InputActionMovementName);
            InputActionRotation = InputActionMap.FindAction(InputActionRotationName);
            InputActionPrimaryAttack = InputActionMap.FindAction(InputActionPrimaryAttackName);
            InputActionBoost = InputActionMap.FindAction(InputActionBoostName);

            InputActionMovement.Enable();
            InputActionRotation.Enable();
            InputActionPrimaryAttack.Enable();
            InputActionBoost.Enable();
        }

        void Update()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var mousePositionWorld = (Vector2)Camera.ScreenToWorldPoint(mousePosition);
            InputAimPosition = mousePositionWorld;
        }

        void OnMovementStarted(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();
            InputMovement = value;
        }

        void OnRotationStarted(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();
            InputRotation = value;
        }
        
        void OnPrimaryAttackStarted(InputAction.CallbackContext context)
        {
            InputPrimaryAttack = true;
        }

        void OnBoostStarted(InputAction.CallbackContext context)
        {
            InputBoost = true;
        }

        void OnMovementCanceled(InputAction.CallbackContext context)
        {
            InputMovement = 0;
        }

        void OnRotationCanceled(InputAction.CallbackContext context)
        {
            InputRotation = 0;
        }

        void OnPrimaryAttackCanceled(InputAction.CallbackContext context)
        {
            InputPrimaryAttack = false;
        }
        
        void OnBoostCanceled(InputAction.CallbackContext context)
        {
            InputBoost = false;
        }
    }
}