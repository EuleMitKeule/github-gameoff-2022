using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.Input;

namespace WorkingTitle.Unity.Components.Physics
{
    [RequireComponent(typeof(TankComponent))]
    [RequireComponent(typeof(InputComponent))]
    public class TankCannonComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        TankCannonAsset TankCannonAsset { get; set; }
        
        TankComponent TankComponent { get; set; }
        
        InputComponent InputComponent { get; set; }

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
                var angle = TankComponent.TankCannon.transform.rotation.eulerAngles.z + TankCannonAsset.RotationSpeed * InputComponent.InputAimRotation * Time.fixedDeltaTime;
                TankComponent.TankCannon.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}