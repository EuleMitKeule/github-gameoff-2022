using UnityEngine;

namespace WorkingTitle.Unity.Components.Ai
{
    public class AiFollowAndAttackStateComponent : AiStateComponent
    {
        public override void StateUpdate()
        {
            if (!AiComponent.IsPlayerVisible)
            {
                SetState<AiFollowStateComponent>();
                return;
            }

            if (AiComponent.IsWithinTargetDistanceThreshold)
            {
                SetState<AiStopAndAttackStateComponent>();
                return;
            }

            AiInputComponent.InputRotation = AiComponent.IsWithinTargetDirectionThreshold ? 0 : Mathf.Sign(AiComponent.PathAngle);
            AiInputComponent.InputMovement = 1;
            AiInputComponent.InputAimDirection = AiComponent.TargetDirection;
            AiInputComponent.InputPrimaryAttack = AiComponent.IsWithinAimThreshold;
        }
    }
}