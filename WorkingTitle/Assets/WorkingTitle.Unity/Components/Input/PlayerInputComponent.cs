using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkingTitle.Unity.Assets;

namespace WorkingTitle.Unity.Components.Input
{
    public class PlayerInputComponent : InputComponent
    {
        [OdinSerialize]
        PlayerInputAsset PlayerInputAsset { get; set; }

        public override AimMode SelectedAimMode => AimMode.Directional;
        
        InputActionMap InputActionMap { get; set; }
        InputAction InputActionMovement { get; set; }
        InputAction InputActionRotation { get; set; }
        InputAction InputActionPrimaryAttack { get; set; }
        InputAction InputActionBoost { get; set; }

        UnityEngine.Camera Camera { get; set; }

        void Start()
        {
            InitializeActions();
            Camera = FindObjectOfType<Camera>();
            EnableInput();
        }

        void InitializeActions()
        {
            InputActionMap = PlayerInputAsset.InputActionAsset.FindActionMap(PlayerInputAsset.InputActionMapName);
            InputActionMovement = InputActionMap.FindAction(PlayerInputAsset.InputActionMovementName);
            InputActionRotation = InputActionMap.FindAction(PlayerInputAsset.InputActionRotationName);
            InputActionPrimaryAttack = InputActionMap.FindAction(PlayerInputAsset.InputActionPrimaryAttackName);
            InputActionBoost = InputActionMap.FindAction(PlayerInputAsset.InputActionBoostName);
            
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