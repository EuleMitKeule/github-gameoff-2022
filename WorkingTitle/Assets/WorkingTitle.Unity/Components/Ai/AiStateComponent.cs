using UnityEngine;
using WorkingTitle.Unity.Components.Input;

namespace WorkingTitle.Unity.Components.Ai
{
    [RequireComponent(typeof(AiComponent))]
    [RequireComponent(typeof(AiInputComponent))]
    public abstract class AiStateComponent : StateComponent
    {
        protected AiComponent AiComponent { get; private set; }
        protected AiInputComponent AiInputComponent { get; private set; }
        protected EnemyTankComponent TankComponent { get; private set; }
        
        void Awake()
        {
            AiComponent = GetComponent<AiComponent>();
            AiInputComponent = GetComponent<AiInputComponent>();
            TankComponent = GetComponent<EnemyTankComponent>();
        }
    }
}