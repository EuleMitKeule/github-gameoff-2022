using System;
using Cinemachine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkingTitle.Unity.Gameplay;

namespace WorkingTitle.Unity.Graphics
{
    public class CameraMouseFollowComponent : SerializedMonoBehaviour
    {
        [OdinSerialize] 
        float MaxDistance { get; set; }
    
        [OdinSerialize] 
        float MouseFollowDamping { get; set; }
    
        [OdinSerialize] 
        CorrectionMode CameraCorrectionMode { get; set; }
        
        GameObject VirtualTargetObject { get; set; }
        
        Camera Camera { get; set; }
        CinemachineVirtualCamera VirtualCamera { get; set; }
        GameComponent GameComponent { get; set; }

        enum CorrectionMode
        {
            None,
            CorrectAspectRatio,
            MinDimension
        }
    
        void Awake()
        {
            Camera = GetComponent<Camera>();
            GameComponent = GetComponentInParent<GameComponent>();

            InitializeVirtualTarget();
        }

        void InitializeVirtualTarget()
        {
            VirtualTargetObject = new GameObject("virtual_camera_target");
            VirtualTargetObject.transform.SetParent(transform);

            VirtualCamera = GetComponent<CinemachineVirtualCamera>();
            VirtualCamera.Follow = VirtualTargetObject.transform;
        }

        void Update()
        {
            var mousePosition = Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var playerPosition = GameComponent.PlayerObject.transform.position;
            var minScreenDim = float.PositiveInfinity;
        
            if (CameraCorrectionMode == CorrectionMode.MinDimension)
            {
                var screenSize = Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
                minScreenDim = Math.Min(screenSize.x, screenSize.y);
            }
        
            var maxDistance = Math.Min(MaxDistance, minScreenDim / 2);
            var direction = (mousePosition - playerPosition).normalized;
            var distance = Math.Min((mousePosition - playerPosition).magnitude, maxDistance);
            var virtualTargetPosition = playerPosition + MouseFollowDamping * distance * direction;

            if (CameraCorrectionMode == CorrectionMode.CorrectAspectRatio)
            {
                virtualTargetPosition.y *= (float) Screen.height / Screen.width;
            }

            VirtualTargetObject.transform.position = virtualTargetPosition;
        }
    }
}
