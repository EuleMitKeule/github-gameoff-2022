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
            mapComponent = MapObject.GetComponent<MapComponent>();

            var centerPosition = new Vector2(mapComponent.MapAsset.ChunkSize / 2f, mapComponent.MapAsset.ChunkSize / 2f);
            PlayerObject = playerComponent ? playerComponent.gameObject : Instantiate(GameAsset.PlayerPrefab, centerPosition, Quaternion.identity, MapObject.transform);
        }
    }
}