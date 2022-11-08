using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WorkingTitle.Unity.Gameplay
{
    public class StatisticsComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        Dictionary<TankAsset, int> KillCounts { get; set; } = new();
        
        [ShowInInspector]
        Dictionary<TankAsset, float> DamageDone { get; set; } = new();
        
        [ShowInInspector]
        float DamageTaken { get; set; }
        
        [ShowInInspector]
        float HealthRecovered { get; set; }
        
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
            
            var tankComponent = healthComponent.GetComponent<TankComponent>();
            if (!tankComponent) return;
            
            var tankAsset = tankComponent.TankAsset;
            
            if (!KillCounts.ContainsKey(tankAsset)) KillCounts[tankAsset] = 0;
            KillCounts[tankAsset] += 1;
        }

        void OnEnemyHealthChanged(object sender, HealthChangedEventArgs e)
        {
            if (e.HealthChange >= 0) return;
            if (sender is not HealthComponent healthComponent) return;
            
            var tankComponent = healthComponent.GetComponent<TankComponent>();
            if (!tankComponent) return;
            
            var tankAsset = tankComponent.TankAsset;
            
            if (!DamageDone.ContainsKey(tankAsset)) DamageDone[tankAsset] = 0;
            DamageDone[tankAsset] += 1;
        }

        void OnPlayerHealthChanged(object sender, HealthChangedEventArgs e)
        {
            if (e.HealthChange < 0)
            {
                DamageTaken += e.HealthChange;
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