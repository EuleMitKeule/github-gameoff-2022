using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay
{
    public class DifficultyComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        public float Difficulty { get; private set; } = 1f;
        
        [OdinSerialize]
        [SuffixLabel("%/s")]
        float DifficultyScaling { get; set; }
        
        void Update()
        {
            Difficulty *= 1 + DifficultyScaling / 100 * Time.deltaTime;
        }
    }
}