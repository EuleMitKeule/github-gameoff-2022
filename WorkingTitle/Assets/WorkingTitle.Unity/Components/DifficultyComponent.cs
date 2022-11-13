using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;

namespace WorkingTitle.Unity.Components
{
    public class DifficultyComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        DifficultyAsset DifficultyAsset { get; set; }
        
        [ShowInInspector]
        [ReadOnly]
        public float Difficulty { get; private set; } = 1f;
        
        void Update()
        {
            Difficulty *= 1 + DifficultyAsset.DifficultyScaling / 100 * Time.deltaTime;
        }
    }
}