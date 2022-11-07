using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace WorkingTitle.Unity
{
    public class StateContextComponent<TBase> : SerializedMonoBehaviour where TBase : StateComponent
    {
        [OdinSerialize]
        [Required]
        [ValueDropdown(nameof(StateComponentValues))]
        TBase InitialStateComponent { get; set; }
        
        protected TBase StateComponent { get; private set; }
        protected IEnumerable<TBase> StateComponents { get; private set; }

        IEnumerable<ValueDropdownItem> StateComponentValues =>
            GetComponents<TBase>().Select(e => new ValueDropdownItem(e.GetType().Name, e));
        
        void Awake()
        {
            StateComponents = GetComponents<TBase>().ToList();

            SetState(InitialStateComponent.GetType());
        }

        void Update()
        {
            if (StateComponent)
            {
                StateComponent.StateUpdate();
            }
        }

        protected void SetState(Type stateType)
        {
            if (!stateType.IsSubclassOf(typeof(TBase))) return;

            var nextState = StateComponents.First(e => e.GetType() == stateType);
            if (!nextState) return;

            if (StateComponent)
            {
                StateComponent.Exit();
                StateComponent.ChangeRequested -= OnChangeRequested;
            }
            
            StateComponent = nextState;
            StateComponent.ChangeRequested += OnChangeRequested;
            StateComponent.Enter();
        }
        
        protected void SetState<T>() where T : TBase => 
            SetState(typeof(T));
        
        void OnChangeRequested(object sender, Type stateType) =>
            SetState(stateType);
    }
}