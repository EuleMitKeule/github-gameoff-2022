using System;
using Cinemachine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkingTitle.Unity.Assets;

namespace WorkingTitle.Unity.Components.Rendering
{
    public class CameraMouseFollowComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        CameraAsset CameraAsset { get; set; }
        
        GameObject VirtualTargetObject { get; set; }
        
        UnityEngine.Camera Camera { get; set; }
        CinemachineVirtualCamera VirtualCamera { get; set; }
        GameComponent GameComponent { get; set; }

    
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
        
            if (CameraAsset.CameraCorrectionMode == CameraCorrectionMode.MinDimension)
            {
                var height = Camera.orthographicSize * 2.0f;
                var width = height * Screen.width / Screen.height;
                var screenSize = new Vector2(width, height);
                minScreenDim = Math.Min(screenSize.x, screenSize.y);
            }
        
            var maxDistance = Math.Min(CameraAsset.MaxDistance, minScreenDim / 2);
            var direction = (mousePosition - playerPosition).normalized;
            var distance = Math.Min((mousePosition - playerPosition).magnitude, maxDistance);
            var virtualTargetPosition = playerPosition + CameraAsset.MouseFollowDamping * distance * direction;

            if (CameraAsset.CameraCorrectionMode == CameraCorrectionMode.CorrectAspectRatio)
            {
                virtualTargetPosition.y *= (float) Screen.height / Screen.width;
            }

            VirtualTargetObject.transform.position = virtualTargetPosition;
        }
    }
}
