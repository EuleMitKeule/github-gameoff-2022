using System;
using Sirenix.Serialization;

namespace TanksOnAPlain.Unity.Components.Difficulty
{
    [Serializable]
    public struct ScaledInt
    {
        [OdinSerialize]
        public int StartValue { get; set; }
        
        [OdinSerialize]
        public int EndValue { get; set; }
        
        [OdinSerialize]
        public int Time { get; set; }
    }
}