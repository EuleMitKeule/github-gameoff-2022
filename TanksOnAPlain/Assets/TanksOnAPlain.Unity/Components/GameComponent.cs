using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets;
using TanksOnAPlain.Unity.Components.Health;
using TanksOnAPlain.Unity.Components.Pooling;
using TanksOnAPlain.Unity.Components.Sound;
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
        SoundComponent SoundComponent { get; set; }
        
        void Awake()
        {
            PoolComponent = FindObjectOfType<PoolComponent>();
            UiComponent = GetComponentInChildren<UiComponent>();
            SoundComponent = GetComponentInChildren<SoundComponent>();
            
            MapObject = Instantiate(GameAsset.MapPrefab, transform);
            PlayerObject = PoolComponent.Allocate(GameAsset.PlayerPrefab);

            SoundComponent.PlayClip(SoundId.Startup);
            
            var playerHealthComponent = PlayerObject.GetComponent<HealthComponent>();
            playerHealthComponent.Death += OnPlayerDeath;
        }

        void OnPlayerDeath(object sender, EventArgs e)
        {
            SoundComponent.PlayClip(SoundId.Death);
            Time.timeScale = 0f;
            UiComponent.ShowGameOver();
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 0f;
        }
    }
}