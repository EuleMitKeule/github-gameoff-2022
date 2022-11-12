using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Map;
using WorkingTitle.Unity.Physics;
using Random = UnityEngine.Random;

namespace WorkingTitle.Unity.Gameplay.Spawning
{
    public class SpawnerComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        float SpawnRadius { get; set; }
        
        [OdinSerialize]
        SpawnTableAsset SpawnTableAsset { get; set; }
        
        [OdinSerialize]
        [MinValue(0)]
        float MaxSpawnCooldown { get; set; }
        
        [OdinSerialize]
        [MinValue(0)]
        float MinSpawnCooldown { get; set; }
        
        [OdinSerialize]
        [MinValue(0)]
        float SpawnCooldownDifficultyModifier { get; set; }
        
        float LastSpawnTime { get; set; }
        
        float SpawnCooldown { get; set; }

        PathfindingComponent PathfindingComponent { get; set; }
        EntityComponent PlayerEntityComponent { get; set; }
        DifficultyComponent DifficultyComponent { get; set; }
        GameComponent GameComponent { get; set; }
        
        public event EventHandler<EnemySpawnedEventArgs> EnemySpawned;
        
        void Start()
        {
            PlayerEntityComponent = GetComponentInChildren<EntityComponent>();
            PathfindingComponent = GetComponentInChildren<PathfindingComponent>();
            DifficultyComponent = GetComponentInParent<DifficultyComponent>();
            GameComponent = GetComponentInParent<GameComponent>();
        }
        
        void Update()
        {
            if (Time.time - LastSpawnTime > SpawnCooldown)
            {
                SpawnEnemy();

                SpawnCooldown = CalculateSpawnCooldown();
                LastSpawnTime = Time.time;
            }
        }
        
        void SpawnEnemy()
        {
            var enemyPrefab = GetRandomEnemy();
            var position = GetRandomPosition();
            
            var enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            enemy.transform.SetParent(transform);
            
            var healthComponent = enemy.GetComponent<HealthComponent>();
            if (healthComponent) healthComponent.MaxHealth *= DifficultyComponent.Difficulty;

            EnemySpawned?.Invoke(this, new EnemySpawnedEventArgs(enemy));
        }
        
        GameObject GetRandomEnemy()
        {
            var random = Random.Range(0f, 1f);
            
            var availableEntries = SpawnTableAsset.SpawnTable
                .Where(pair => pair.Value.MinSpawnDifficulty <= DifficultyComponent.Difficulty)
                .ToList();

            var weights = availableEntries
                .Select(pair => new KeyValuePair<SkillType, float>(pair.Key, pair.Value.CalculateSpawnWeight(DifficultyComponent.Difficulty)))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            var weightSum = weights.Sum(pair => pair.Value);

            var runningWeight = 0f;
            
            foreach (var (skillType, weight) in weights)
            {
                var scaledWeight = weight / weightSum;
                runningWeight += scaledWeight;
                
                if (runningWeight >= random)
                {
                    return GameComponent.SkillAssets[skillType].TankPrefab;
                }
            }
            
            return null;
        }
        
        Vector3 GetRandomPosition()
        {
            var positionsInRadius = new List<Vector2Int>();

            for (var x = (int)-SpawnRadius; x < SpawnRadius; x++)
            {
                for (var y = (int)-SpawnRadius; y < SpawnRadius; y++)
                {
                    var deltaPosition = new Vector2Int(x, y);
                    if (deltaPosition.magnitude > SpawnRadius) continue;

                    var position = PlayerEntityComponent.CellPosition + deltaPosition;
                    var cell = PathfindingComponent.GetCell(position);

                    if (cell is null) continue;
                    if (cell.IsObstacle) continue;
                    
                    positionsInRadius.Add(position);
                }
            }
            
            var randomIndex = Random.Range(0, positionsInRadius.Count);
            var randomPosition = positionsInRadius[randomIndex];
            
            return randomPosition.ToWorld();
        }

        float CalculateSpawnCooldown() => 
            MaxSpawnCooldown - 2 * (MaxSpawnCooldown - MinSpawnCooldown) / Mathf.PI *
            Mathf.Atan(SpawnCooldownDifficultyModifier * DifficultyComponent.Difficulty);
    }
}