using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.Physics;
using Vector2 = UnityEngine.Vector2;

namespace WorkingTitle.Unity.Components.Input
{
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(EnemyTankComponent))]
    public class AiInputComponent : InputComponent
    {
        [OdinSerialize]
        AiInputAsset AiInputAsset { get; set; }

        public override AimMode SelectedAimMode => AimMode.Rotational;
        
        EnemyTankComponent TankComponent { get; set; }

        void Awake()
        {
            TankComponent = GetComponent<EnemyTankComponent>();
        }

        void Update()
        {
            var currentAimDirection = TankComponent.TankCannon.transform.up;
            var angle = Vector2.SignedAngle(currentAimDirection, InputAimDirection);
            var rotationSign = (int)Mathf.Sign(angle);
            InputAimRotation = Mathf.Abs(angle) > AiInputAsset.AimRotationThreshold ? rotationSign : 0;
        }
        
        public override void EnableInput()
        {
            throw new System.NotImplementedException();
        }

        public override void DisableInput()
        {
            throw new System.NotImplementedException();
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!TankComponent) return;
            if (!TankComponent.TankCannon) return;
            var currentAimDirection = TankComponent.TankCannon.transform.up;
            Debug.DrawRay(transform.position, currentAimDirection, Color.red);
            Debug.DrawRay(transform.position, InputAimDirection, Color.green);
        }
#endif
    }
}