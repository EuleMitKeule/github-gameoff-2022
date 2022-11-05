using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using WorkingTitle.Unity.Gameplay;

public class CameraMouseFollowComponent : SerializedMonoBehaviour
{
    
    GameObject VirtualTargetObject { get; set; }
    
    Camera Camera { get; set; }

    CinemachineVirtualCamera VirtualCamera { get; set; }

    [OdinSerialize]
    GameObject PlayerObject { get; set; }
    
    [OdinSerialize] float MaxDistance { get; set; }
    
    [OdinSerialize] float MouseFollowDamping { get; set; }
    
    [OdinSerialize] CorrectionMode CameraCorrectionMode { get; set; }


    enum CorrectionMode
    {
        None,
        CorrectAspectRatio,
        MinDimension
    }
    
    void Awake()
    {
        Camera = GetComponent<Camera>();

        PlayerObject = FindObjectOfType<PlayerComponent>().gameObject;

        VirtualTargetObject = new GameObject("virtual_camera_target");
        VirtualTargetObject.transform.SetParent(transform);

        VirtualCamera = GetComponent<CinemachineVirtualCamera>();
        VirtualCamera.Follow = VirtualTargetObject.transform;

    }

    private void Update()
    {
        var mousePosition = Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var playerPosition = PlayerObject.transform.position;

        var minScreenDim = float.PositiveInfinity;
        
        if (CameraCorrectionMode == CorrectionMode.MinDimension)
        {
            var screenSize = Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        
            minScreenDim = Math.Min(
                screenSize.x,
                screenSize.y
            );
        }
        
        var maxDistance = Math.Min(
            MaxDistance,
            minScreenDim / 2);

        var direction = (mousePosition - playerPosition).normalized;
        var distance = Math.Min(
            (mousePosition - playerPosition).magnitude,
            maxDistance
        );

        var virtualTargetPosition = playerPosition + MouseFollowDamping * distance * direction;

        if (CameraCorrectionMode == CorrectionMode.CorrectAspectRatio)
        {
            virtualTargetPosition.y *= (float) Screen.height / Screen.width;
        }

        VirtualTargetObject.transform.position = virtualTargetPosition;

    }
}
