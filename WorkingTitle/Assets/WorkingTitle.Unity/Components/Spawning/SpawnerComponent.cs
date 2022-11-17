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
        
        [OdinSerialize]
        int SpawnRadiusOffset { get; set; }
        
        float LastSpawnTime { get; set; }
        
        float SpawnCooldown { get; set; }
        
        int EnemyCount { get; set; }

        PathfindingComponent PathfindingComponent { get; set; }
        EntityComponent PlayerEntityComponent { get; set; }
        DifficultyComponent DifficultyComponent { get; set; }
        GameComponent GameComponent { get; set; }
        Camera Camera { get; set; }
        
        public event EventHandler<EnemySpawnedEventArgs> EnemySpawned;
        
        void Start()
        {
            EnemyCount = 0;
            
            PlayerEntityComponent = GetComponentInChildren<EntityComponent>();
            PathfindingComponent = GetComponentInChildren<PathfindingComponent>();
            DifficultyComponent = GetComponentInParent<DifficultyComponent>();
            GameComponent = GetComponentInParent<GameComponent>();
            Camera = FindObjectOfType<Camera>();
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
            var cellPosition = position.ToCell();
            var cell = PathfindingComponent.GetCell(cellPosition);
            var direction = cell.Direction;
            var angle = Vector2.SignedAngle(Vector2.up, direction);
            var rotation = Quaternion.Euler(0, 0, angle);;
            
            var enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            enemy.transform.SetParent(transform);

            var tankComponent = enemy.GetComponent<TankComponent>();
            if (tankComponent)
                tankComponent.TankBody.transform.rotation = rotation;
            
            var spriteRenderers = enemy.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder = EnemyCount;
            }

            EnemyCount += 1;
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
        
        Vector2 GetRandomPosition()
        {
            var diagonal = (int)Camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).magnitude;
            var spawnRadius = diagonal / 2 + SpawnRadiusOffset;

            var cellPositions = new List<Vector2Int>();
            
            for (int x = -spawnRadius - 1; x <= spawnRadius + 1; x++)
            {
                for (int y = -spawnRadius - 1; y <= spawnRadius + 1; y++)
                {
                    var cellPosition = new Vector2Int(x, y);
                    var distance = cellPosition.magnitude;
                    
                    if(distance < spawnRadius || distance > spawnRadius + 1)
                        continue;
                    
                    cellPositions.Add(cellPosition);
                }
            }

            var chosenCellPosition = Vector2Int.zero;

            while (cellPositions.Count > 0)
            {
                var randomIndex = Random.Range(0, cellPositions.Count);
                chosenCellPosition = cellPositions[randomIndex];
                
                var worldCellPosition =
                    new Vector2Int((int)PlayerEntityComponent.Position.x, (int)PlayerEntityComponent.Position.y) +
                    chosenCellPosition;
                var cell = PathfindingComponent.GetCell(worldCellPosition);

                if (!(cell?.IsObstacle ?? true)) break;
                
                cellPositions.Remove(chosenCellPosition);
            }

            return (PlayerEntityComponent.CellPosition + chosenCellPosition).ToWorld();
        }

        float CalculateSpawnCooldown() => 
            SpawnerAsset.MaxSpawnCooldown - 2 * (SpawnerAsset.MaxSpawnCooldown - SpawnerAsset.MinSpawnCooldown) / Mathf.PI *
            Mathf.Atan(SpawnerAsset.SpawnCooldownDifficultyModifier * DifficultyComponent.Difficulty);
    }
}