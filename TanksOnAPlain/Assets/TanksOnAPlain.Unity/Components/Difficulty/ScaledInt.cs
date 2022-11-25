using System;
using Sirenix.Serialization;

namespace TanksOnAPlain.Unity.Components.Difficulty
{
    [Serializable]
    public struct ScaledInt
    {
        [OdinSerialize]
        public int MinValue { get; set; }
        
        [OdinSerialize]
        public int MaxValue { get; set; }
        
        [OdinSerialize]
        public int Time { get; set; }
    }
}