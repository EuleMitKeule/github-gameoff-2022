using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Assets.PowerUps;
using WorkingTitle.Unity.Components.Health;
using WorkingTitle.Unity.Components.PowerUps;
using WorkingTitle.Unity.Components.Spawning;

namespace WorkingTitle.Unity.Components
{
    public class StatisticsComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        public Dictionary<TankAsset, int> KillCounts { get; set; } = new();
        
        [ShowInInspector]
        public Dictionary<TankAsset, float> DamageDone { get; set; } = new();
        
        [ShowInInspector]
        public float DamageTaken { get; set; }
        
        [ShowInInspector]
        public float HealthRecovered { get; set; }
        
        float StartTime { get; set; }
        float TimeSurvived { get; set; }
        
        SpawnerComponent SpawnerComponent { get; set; }
        GameComponent GameComponent { get; set; }
        
        void Start()
        {
            SpawnerComponent = GetComponentInChildren<SpawnerComponent>();
            GameComponent = GetComponent<GameComponent>();
            
            var playerHealthComponent = GameComponent.PlayerObject.GetComponent<HealthComponent>();

            StartTime = Time.time;

            SpawnerComponent.EnemySpawned += OnEnemySpawned;
            playerHealthComponent.HealthChanged += OnPlayerHealthChanged;
            playerHealthComponent.Death += OnPlayerDeath;
        }

        void OnEnemySpawned(object sender, EnemySpawnedEventArgs e)
        {
            var healthComponent = e.Enemy.GetComponent<HealthComponent>();
            if (!healthComponent) return;
            
            healthComponent.Death += OnEnemyDeath;
            healthComponent.HealthChanged += OnEnemyHealthChanged;
        }

        void OnEnemyDeath(object sender, EventArgs e)
        {
            if (sender is not HealthComponent healthComponent) return;
            
            var tankComponent = healthComponent.GetComponent<EnemyTankComponent>();
            if (!tankComponent) return;
            
            var tankAsset = tankComponent.TankAsset;
            
            if (!KillCounts.ContainsKey(tankAsset)) KillCounts[tankAsset] = 0;
            KillCounts[tankAsset] += 1;
        }

        void OnEnemyHealthChanged(object sender, HealthChangedEventArgs e)
        {
            if (e.HealthChange >= 0) return;
            if (sender is not HealthComponent healthComponent) return;
            
            var tankComponent = healthComponent.GetComponent<EnemyTankComponent>();
            if (!tankComponent) return;
            
            var tankAsset = tankComponent.TankAsset;
            
            if (!DamageDone.ContainsKey(tankAsset)) DamageDone[tankAsset] = 0;
            DamageDone[tankAsset] += -e.HealthChange;
        }

        void OnPlayerHealthChanged(object sender, HealthChangedEventArgs e)
        {
            if (e.HealthChange < 0)
            {
                DamageTaken += -e.HealthChange;
            }
            else
            {
                HealthRecovered += e.HealthChange;
            }
        }

        void OnPlayerDeath(object sender, EventArgs e)
        {
            TimeSurvived = Time.time - StartTime;
        }
    }
}