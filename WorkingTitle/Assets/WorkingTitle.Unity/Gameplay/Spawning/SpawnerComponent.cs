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
    [RequireComponent(typeof(PathfindingComponent))]
    public class SpawnerComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        float SpawnRadius { get; set; }
        
        [OdinSerialize]
        float SpawnCooldown { get; set; }

        [OdinSerialize]
        // [ValidateInput(nameof(IsEnemySpawnTableNotEmpty), "Enemy Spawn Table must contain at least one entry")]
        // [ValidateInput(nameof(IsEnemySpawnTableNotContainsNull), "Enemy Spawn Table must not contain null entries")]
        // [ValidateInput(nameof(IsEnemySpawnTableWeightsSumOne), "Enemy Spawn Table weights must sum up to 1")]
        // [ValidateInput(nameof(IsEnemySpawnTableNotContainsZero), "Enemy Spawn Table must not contain zero weights")]
        Dictionary<GameObject, float> EnemySpawnTable { get; set; } = new();
        
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

        PathfindingComponent PathfindingComponent { get; set; }
        EntityComponent PlayerEntityComponent { get; set; }
        DifficultyComponent DifficultyComponent { get; set; }
        
        public event EventHandler<EnemySpawnedEventArgs> EnemySpawned;
        
        # region Editor
        
        // bool IsEnemySpawnTableNotEmpty => EnemySpawnTable is null || EnemySpawnTable.Count > 0;
        // bool IsEnemySpawnTableWeightsSumOne => EnemySpawnTable is null || Math.Abs(EnemySpawnTable.Values.Sum() - 1) < float.Epsilon;
        // bool IsEnemySpawnTableNotContainsNull => EnemySpawnTable?.Keys.All(key => key != null) ?? true;
        // bool IsEnemySpawnTableNotContainsZero => EnemySpawnTable?.Values.All(value => value != 0) ?? true;
        
        # endregion

        void Awake()
        {
            PathfindingComponent = GetComponent<PathfindingComponent>();
            DifficultyComponent = GetComponentInParent<DifficultyComponent>();
        }
        
        void Start()
        {
            PlayerEntityComponent = GetComponentInChildren<EntityComponent>();
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
            
            var healthComponent = GetComponent<HealthComponent>();
            healthComponent.MaxHealth *= DifficultyComponent.Difficulty;

            EnemySpawned?.Invoke(this, new EnemySpawnedEventArgs(enemy));
        }
        
        GameObject GetRandomEnemy()
        {
            var random = Random.Range(0f, 1f);
            var sum = 0f;
            
            
            foreach (var entry in EnemySpawnTable)
            {
                sum += entry.Value;
                
                if (random <= sum)
                {
                    return entry.Key;
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