using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.Spawning
{
    [Serializable]
    public class SpawnTableEntry
    {
        [OdinSerialize]
        public float MinSpawnDifficulty { get; set; }
        
        [OdinSerialize]
        [ShowIf(nameof(IsWeightModeFixed))]
        float SpawnWeight { get; set; }
        
        [OdinSerialize]
        [ShowIf(nameof(IsWeightModeScaled))]
        float MinWeight { get; set; }
        
        [OdinSerialize]
        [ShowIf(nameof(IsWeightModeScaled))]
        float MaxWeight { get; set; }
        
        [OdinSerialize]
        [ShowIf(nameof(IsWeightModeScaled))]
        float WeightModifier { get; set; }

        [OdinSerialize]
        WeightMode SelectedWeightMode { get; set; }
        
        bool IsWeightModeFixed => SelectedWeightMode == WeightMode.Fixed;
        bool IsWeightModeScaled => SelectedWeightMode == WeightMode.Scaled;

        public enum WeightMode
        {
            Fixed,
            Scaled
        }

        public float CalculateSpawnWeight(float difficulty)
        {
            if (IsWeightModeFixed)
            {
                return SpawnWeight;
            }
            
            return MaxWeight - 2 * (MaxWeight - MinWeight) / Mathf.PI *
                Mathf.Atan(WeightModifier * difficulty);
        }
    }
}