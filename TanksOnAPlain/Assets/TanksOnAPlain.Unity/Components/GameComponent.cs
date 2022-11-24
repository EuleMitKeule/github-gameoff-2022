using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets;
using TanksOnAPlain.Unity.Components.Health;
using TanksOnAPlain.Unity.Components.Pooling;
using TanksOnAPlain.Unity.Components.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TanksOnAPlain.Unity.Components
{
    public class GameComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public GameAsset GameAsset { get; set; }

        public GameObject PlayerObject { get; set; }
        GameObject MapObject { get; set; }
        PoolComponent PoolComponent { get; set; }
        UiComponent UiComponent { get; set; }        
        
        void Awake()
        {
            PoolComponent = FindObjectOfType<PoolComponent>();
            UiComponent = GetComponentInChildren<UiComponent>();
            
            MapObject = Instantiate(GameAsset.MapPrefab, transform);
            PlayerObject = PoolComponent.Allocate(GameAsset.PlayerPrefab);

            var playerHealthComponent = PlayerObject.GetComponent<HealthComponent>();
            playerHealthComponent.Death += OnPlayerDeath;
        }

        void OnPlayerDeath(object sender, EventArgs e)
        {
            Time.timeScale = 0f;
            UiComponent.ShowGameOver();
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1f;
        }
    }
}