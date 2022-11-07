using System.Collections.Generic;
using System.Linq;
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
        [ValueDropdown(nameof(InputActionMapNameValues))]
        [Required]
        string InputActionMapName { get; set; }
        
        [OdinSerialize]
        [ValueDropdown(nameof(InputActionNameValues))]
        [Required]
        string InputActionMovementName { get; set; }
        
        [OdinSerialize]
        [ValueDropdown(nameof(InputActionNameValues))]
        [Required]
        string InputActionRotationName { get; set; }
        
        [OdinSerialize]
        [ValueDropdown(nameof(InputActionNameValues))]
        [Required]
        string InputActionPrimaryAttackName { get; set; }
        
        [OdinSerialize]
        [ValueDropdown(nameof(InputActionNameValues))]
        [Required]
        string InputActionBoostName { get; set; }

        public override AimMode SelectedAimMode => AimMode.Directional;
        
        InputActionMap InputActionMap { get; set; }
        InputAction InputActionMovement { get; set; }
        InputAction InputActionRotation { get; set; }
        InputAction InputActionPrimaryAttack { get; set; }
        InputAction InputActionBoost { get; set; }

        Camera Camera { get; set; }

        # region Editor
        
        IEnumerable<string> InputActionMapNameValues => 
            InputActionAsset.actionMaps.Select(e => e.name);

        IEnumerable<string> InputActionNameValues =>
            InputActionMap?.actions.Select(e => e.name);
        
        # endregion
        
        void Awake()
        {
            Camera = Camera.main;

            InitializeActions();
            
            EnableInput();
        }

        void InitializeActions()
        {
            InputActionMap = InputActionAsset.FindActionMap(InputActionMapName);
            InputActionMovement = InputActionMap.FindAction(InputActionMovementName);
            InputActionRotation = InputActionMap.FindAction(InputActionRotationName);
            InputActionPrimaryAttack = InputActionMap.FindAction(InputActionPrimaryAttackName);
            InputActionBoost = InputActionMap.FindAction(InputActionBoostName);
            
            InputActionMovement.started += OnMovementStarted;
            InputActionRotation.started += OnRotationStarted;
            InputActionPrimaryAttack.started += OnPrimaryAttackStarted;
            InputActionBoost.started += OnBoostStarted;
            InputActionMovement.canceled += OnMovementCanceled;
            InputActionRotation.canceled += OnRotationCanceled;
            InputActionPrimaryAttack.canceled += OnPrimaryAttackCanceled;
            InputActionBoost.canceled += OnBoostCanceled;
        }

        public override void EnableInput()
        {
            InputActionMovement.Enable();
            InputActionRotation.Enable();
            InputActionPrimaryAttack.Enable();
            InputActionBoost.Enable();
        }
        
        public override void DisableInput()
        {
            InputActionMovement.Disable();
            InputActionRotation.Disable();
            InputActionPrimaryAttack.Disable();
            InputActionBoost.Disable();
        }

        void Update()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var mousePositionWorld = (Vector2)Camera.ScreenToWorldPoint(mousePosition);
            InputAimDirection = (mousePositionWorld - (Vector2)transform.position).normalized;
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