using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components
{
    public class DifficultyComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        DifficultyAsset DifficultyAsset { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float Difficulty { get; private set; }
        
        void Update()
        {
            Difficulty += Time.deltaTime;
        }

        public int GetScaledValueExp(int startValue, int endValue, int time)
        {
            var reverse = startValue > endValue;
            var delta = reverse ? 
                startValue - endValue : 
                endValue - startValue;
            var sign = reverse ? -1 : 1;
            
            var modifier = Mathf.Pow(time, 2) / (delta);
            var value =  Mathf.RoundToInt(startValue + sign * Mathf.Pow(Difficulty, 2) / modifier);
            
            return Mathf.Clamp(value,
                reverse ? endValue : startValue, 
                reverse ? startValue : endValue);
        }

        public float GetScaledValueExp(float startValue, float endValue, int time)
        {
            var reverse = startValue > endValue;
            var delta = reverse ? 
                startValue - endValue : 
                endValue - startValue;
            var sign = reverse ? -1 : 1;
            
            var modifier = Mathf.Pow(time, 2) / (delta);
            var value = startValue + sign * Mathf.Pow(Difficulty, 2) / modifier;

            return Mathf.Clamp(value,
                reverse ? endValue : startValue, 
                reverse ? startValue : endValue);
        }
    }
}