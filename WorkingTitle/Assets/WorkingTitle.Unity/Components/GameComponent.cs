using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.Map;
using WorkingTitle.Unity.Components.Pooling;

namespace WorkingTitle.Unity.Components
{
    public class GameComponent : SerializedMonoBehaviour
    {
        [OdinSerialize] public GameAsset GameAsset { get; set; }

        GameObject MapObject { get; set; }
        public GameObject PlayerObject { get; set; }

        PoolComponent PoolComponent { get; set; }
        
        void Awake()
        {
            PoolComponent = FindObjectOfType<PoolComponent>();
            
            MapObject = Instantiate(GameAsset.MapPrefab, transform);
            PlayerObject = PoolComponent.Allocate(GameAsset.PlayerPrefab);
        }
    }
}