using UnityEngine;
using WorkingTitle.Unity.Input;

namespace WorkingTitle.Unity.Gameplay.Ai
{
    [RequireComponent(typeof(AiComponent))]
    [RequireComponent(typeof(AiInputComponent))]
    public abstract class AiStateComponent : StateComponent
    {
        protected AiComponent AiComponent { get; private set; }
        protected AiInputComponent AiInputComponent { get; private set; }
        protected TankComponent TankComponent { get; private set; }
        
        void Awake()
        {
            AiComponent = GetComponent<AiComponent>();
            AiInputComponent = GetComponent<AiInputComponent>();
            TankComponent = GetComponent<TankComponent>();
        }
    }
}