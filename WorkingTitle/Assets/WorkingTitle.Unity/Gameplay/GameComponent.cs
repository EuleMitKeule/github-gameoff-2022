using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Map;

namespace WorkingTitle.Unity.Gameplay
{
    public class GameComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Optional]
        GameObject MapPrefab { get; set; }
        
        [OdinSerialize]
        [Optional]
        GameObject PlayerPrefab { get; set; }

        GameObject MapObject { get; set; }
        public GameObject PlayerObject { get; set; }
        
        void Awake()
        {
            var mapComponent = FindObjectOfType<MapComponent>();
            var playerComponent = FindObjectOfType<PlayerComponent>();
            
            MapObject = mapComponent ? mapComponent.gameObject : Instantiate(MapPrefab, transform);
            PlayerObject = playerComponent ? playerComponent.gameObject : Instantiate(PlayerPrefab, MapObject.transform);
        }
    }
}