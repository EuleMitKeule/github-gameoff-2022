using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.Map;

namespace WorkingTitle.Unity.Components
{
    public class GameComponent : SerializedMonoBehaviour
    {
        [OdinSerialize] public GameAsset GameAsset { get; set; }

        GameObject MapObject { get; set; }
        public GameObject PlayerObject { get; set; }
        
        void Awake()
        {
            var mapComponent = FindObjectOfType<MapComponent>();
            var playerComponent = FindObjectOfType<PlayerComponent>();
            
            MapObject = mapComponent ? mapComponent.gameObject : Instantiate(GameAsset.MapPrefab, transform);

            PlayerObject = playerComponent ? playerComponent.gameObject : Instantiate(GameAsset.PlayerPrefab, MapObject.transform);
        }
    }
}