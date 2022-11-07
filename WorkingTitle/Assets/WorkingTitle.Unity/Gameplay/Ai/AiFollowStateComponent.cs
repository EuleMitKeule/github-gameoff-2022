using Sirenix.OdinInspector;
using UnityEngine;
using WorkingTitle.Unity.Map;

namespace WorkingTitle.Unity.Gameplay.Ai
{
    public class AiFollowStateComponent : AiStateComponent
    {
        public override void StateUpdate()
        {
            if (AiComponent.IsPlayerVisible)
            {
                SetState<AiFollowAndAttackStateComponent>();
                return;
            }

            AiInputComponent.InputRotation = AiComponent.IsWithinTargetDirectionThreshold ? 0 : Mathf.Sign(AiComponent.PathAngle);
            AiInputComponent.InputMovement = 1;
            AiInputComponent.InputAimDirection = TankComponent.TankBody.transform.up;
            AiInputComponent.InputPrimaryAttack = false;
        }
    }
}