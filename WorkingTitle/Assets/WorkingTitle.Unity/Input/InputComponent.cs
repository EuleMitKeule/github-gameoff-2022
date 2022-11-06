using Sirenix.OdinInspector;
using UnityEngine;

namespace WorkingTitle.Unity.Input
{
    public abstract class InputComponent : SerializedMonoBehaviour
    {
        [TitleGroup("Input")]
        [ShowInInspector]
        [ReadOnly]
        [PropertyOrder(-3)]
        public float InputMovement { get; protected set; }
        
        [ShowInInspector]
        [ReadOnly]
        [PropertyOrder(-2)]
        public float InputRotation { get; protected set; }
        
        [ShowInInspector]
        [ReadOnly]
        [PropertyOrder(-1)]
        public Vector2 InputAimPosition { get; protected set; }
                
        [ShowInInspector]
        [ReadOnly]
        [PropertyOrder(0)]
        public bool InputBoost { get; protected set; }
    }
}