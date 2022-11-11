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
        float SpawnInterval { get; set; }

        [OdinSerialize]
        [ValidateInput(nameof(IsEnemySpawnTableNotEmpty), "Enemy Spawn Table must contain at least one entry")]
        [ValidateInput(nameof(IsEnemySpawnTableNotContainsNull), "Enemy Spawn Table must not contain null entries")]
        [ValidateInput(nameof(IsEnemySpawnTableWeightsSumOne), "Enemy Spawn Table weights must sum up to 1")]
        [ValidateInput(nameof(IsEnemySpawnTableNotContainsZero), "Enemy Spawn Table must not contain zero weights")]
        Dictionary<GameObject, float> EnemySpawnTable { get; set; } = new();
        
        float LastSpawnTime { get; set; }

        PathfindingComponent PathfindingComponent { get; set; }
        EntityComponent PlayerEntityComponent { get; set; }
        
        public event EventHandler<EnemySpawnedEventArgs> EnemySpawned;
        
        # region Editor
        
        bool IsEnemySpawnTableNotEmpty => EnemySpawnTable is null || EnemySpawnTable.Count > 0;
        bool IsEnemySpawnTableWeightsSumOne => EnemySpawnTable is null || Math.Abs(EnemySpawnTable.Values.Sum() - 1) < float.Epsilon;
        bool IsEnemySpawnTableNotContainsNull => EnemySpawnTable?.Keys.All(key => key != null) ?? true;
        bool IsEnemySpawnTableNotContainsZero => EnemySpawnTable?.Values.All(value => value != 0) ?? true;
        
        # endregion

        void Awake()
        {
            PathfindingComponent = GetComponent<PathfindingComponent>();
        }
        
        void Start()
        {
            PlayerEntityComponent = GetComponentInChildren<EntityComponent>();
        }
        
        void Update()
        {
            if (Time.time - LastSpawnTime > SpawnInterval)
            {
                SpawnEnemy();
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
    }
}