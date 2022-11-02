using Sirenix.OdinInspector;
using UnityEngine;

namespace WorkingTitle.Unity.Input
{
    public abstract class InputComponent : SerializedMonoBehaviour
    {
        [TitleGroup("Input")]
        [ShowInInspector]
        [ReadOnly]
        public float InputMovement { get; protected set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float InputRotation { get; protected set; }
        
        [ShowInInspector]
        [ReadOnly]
        public Vector2 InputAimPosition { get; protected set; }
    }
}