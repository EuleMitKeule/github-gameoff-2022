
namespace TanksOnAPlain.Unity.Components.Ai
{
    public class AiStopAndAttackStateComponent : AiStateComponent
    {
        public override void StateUpdate()
        {
            if (!AiComponent.IsPlayerVisible)
            {
                SetState<AiFollowStateComponent>();
                return;
            }
            
            if (!AiComponent.IsWithinTargetDistanceThreshold)
            {
                SetState<AiFollowAndAttackStateComponent>();
                return;
            }

            AiInputComponent.InputRotation = 0;
            AiInputComponent.InputMovement = 0;
            AiInputComponent.InputAimDirection = AiComponent.TargetDirection;
            AiInputComponent.InputPrimaryAttack = AiComponent.IsWithinAimThreshold;
        }
    }
}