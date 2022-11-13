using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets.Spawning;
using WorkingTitle.Unity.Components.Map;
using WorkingTitle.Unity.Components.Physics;
using WorkingTitle.Unity.Extensions;
using Random = UnityEngine.Random;

namespace WorkingTitle.Unity.Components.Spawning
{
    public class SpawnerComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        SpawnerAsset SpawnerAsset { get; set; }
        
        [OdinSerialize]
        SpawnTableAsset SpawnTableAsset { get; set; }
        
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
                    return GameComponent.GameAsset.SkillAssets[skillType].TankPrefab;
                }
            }
            
            return null;
        }
        
        Vector3 GetRandomPosition()
        {
            var positionsInRadius = new List<Vector2Int>();

            for (var x = (int)-SpawnerAsset.SpawnRadius; x < SpawnerAsset.SpawnRadius; x++)
            {
                for (var y = (int)-SpawnerAsset.SpawnRadius; y < SpawnerAsset.SpawnRadius; y++)
                {
                    var deltaPosition = new Vector2Int(x, y);
                    if (deltaPosition.magnitude > SpawnerAsset.SpawnRadius) continue;

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
            SpawnerAsset.MaxSpawnCooldown - 2 * (SpawnerAsset.MaxSpawnCooldown - SpawnerAsset.MinSpawnCooldown) / Mathf.PI *
            Mathf.Atan(SpawnerAsset.SpawnCooldownDifficultyModifier * DifficultyComponent.Difficulty);
    }
}