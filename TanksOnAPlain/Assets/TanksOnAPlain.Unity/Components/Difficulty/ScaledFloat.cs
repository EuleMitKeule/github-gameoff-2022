using System;
using Sirenix.Serialization;

namespace TanksOnAPlain.Unity.Components.Difficulty
{
    [Serializable]
    public struct ScaledFloat
    {
        [OdinSerialize]
        public float StartValue { get; set; }
        
        [OdinSerialize]
        public float EndValue { get; set; }
        
        [OdinSerialize]
        public int Time { get; set; }
    }
}