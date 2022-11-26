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
            //Difficulty *= 1 + DifficultyAsset.DifficultyScaling / 100 * Time.deltaTime;
            Difficulty += Time.deltaTime;
        }

        // public float GetScaledValueAtan(float startValue, float endValue, float modifier)
        // {
        //     return startValue + 2 * (endValue - startValue) / Mathf.PI *
        //         Mathf.Atan(modifier * Difficulty);
        // }
        //
        // public int GetScaledValueAtan(int startValue, int endValue, float modifier)
        // {
        //     Debug.Log($"{startValue} -> {endValue}\n=> {Mathf.RoundToInt(startValue + 2 * (endValue - startValue) / Mathf.PI * Mathf.Atan(modifier * Difficulty))}");
        //     return Mathf.RoundToInt(startValue + 2 * (endValue - startValue) / Mathf.PI *
        //         Mathf.Atan(modifier * Difficulty));
        // }

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

            //Debug.Log($"{time}^2 / {delta} = {modifier}\n{startValue} + {sign} * {Difficulty}^2 / {modifier} = {value}");
            return Mathf.Clamp(value,
                reverse ? endValue : startValue, 
                reverse ? startValue : endValue);
        }
    }
}