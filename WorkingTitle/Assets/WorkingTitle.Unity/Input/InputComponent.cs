using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Input
{
    public abstract class InputComponent : SerializedMonoBehaviour
    {
        [TitleGroup("Input")]
        [ShowInInspector]
        [ReadOnly]
        public float InputMovement { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float InputRotation { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public Vector2 InputAimDirection { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float InputAimRotation { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public bool InputPrimaryAttack { get; set; }
                
        [ShowInInspector]
        [ReadOnly]
        public bool InputBoost { get; protected set; }

        public virtual AimMode SelectedAimMode { get; set; }
        
        public enum AimMode
        {
            Rotational,
            Directional
        }

        public abstract void EnableInput();
        public abstract void DisableInput();
    }
}