using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets.Spawning;
using TanksOnAPlain.Unity.Components.Health;
using TanksOnAPlain.Unity.Components.Map.Pathfinding;
using TanksOnAPlain.Unity.Components.Physics;
using TanksOnAPlain.Unity.Components.Pooling;
using TanksOnAPlain.Unity.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TanksOnAPlain.Unity.Components.Spawning
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
        PoolComponent PoolComponent { get; set; }
        Camera Camera { get; set; }
        
        public event EventHandler<EnemySpawnedEventArgs> EnemySpawned;
        
        void Awake()
        {
            EnemyCount = 0;
            
            PathfindingComponent = FindObjectOfType<PathfindingComponent>();
            DifficultyComponent = FindObjectOfType<DifficultyComponent>();
            GameComponent = FindObjectOfType<GameComponent>();
            PoolComponent = FindObjectOfType<PoolComponent>();
            Camera = FindObjectOfType<Camera>();
        }

        void Start()
        {
            PlayerEntityComponent = GameComponent.PlayerObject.GetComponent<EntityComponent>();
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
            
            var enemyObject = PoolComponent.Allocate(enemyPrefab, position);
            if (!enemyObject) return;

            var tankComponent = enemyObject.GetComponent<TankComponent>();
            if (tankComponent)
            {
                tankComponent.TankBody.transform.rotation = rotation;
            }
            
            var spriteRenderers = enemyObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder = EnemyCount;
            }

            EnemyCount += 1;
            EnemySpawned?.Invoke(this, new EnemySpawnedEventArgs(enemyObject));
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
                    return GameComponent.GameAsset.TankAssets[skillType].Prefab;
                }
            }
            
            return null;
        }
        
        Vector2 GetRandomPosition()
        {
            var height = Camera.orthographicSize * 2.0f;
            var width = height * Screen.width / Screen.height;
            var size = new Vector2(width, height);
            
            var diagonal = Mathf.FloorToInt(size.magnitude);
            var spawnRadius = diagonal / 2 + SpawnRadiusOffset;
            
            var relativeCellPositions = new List<Vector2Int>();
            
            for (int x = -spawnRadius - 1; x <= spawnRadius + 1; x++)
            {
                for (int y = -spawnRadius - 1; y <= spawnRadius + 1; y++)
                {
                    var relativeCellPosition = new Vector2Int(x, y);
                    var distance = relativeCellPosition.magnitude;

                    if (distance < spawnRadius || distance > spawnRadius + 1)
                    {
                        continue;
                    }
                    
                    relativeCellPositions.Add(relativeCellPosition);
                }
            }

            var chosenRelativeCellPosition = Vector2Int.zero;

            while (relativeCellPositions.Count > 0)
            {
                var randomIndex = Random.Range(0, relativeCellPositions.Count);
                chosenRelativeCellPosition = relativeCellPositions[randomIndex];
                
                var cellPosition = PlayerEntityComponent.CellPosition + chosenRelativeCellPosition;
                var cell = PathfindingComponent.GetCell(cellPosition);

                if (!(cell?.IsObstacle ?? true)) break;
                
                relativeCellPositions.Remove(chosenRelativeCellPosition);
            }

            return (PlayerEntityComponent.CellPosition + chosenRelativeCellPosition).ToWorld();
        }

        float CalculateSpawnCooldown() => 
            SpawnerAsset.MaxSpawnCooldown - 2 * (SpawnerAsset.MaxSpawnCooldown - SpawnerAsset.MinSpawnCooldown) / Mathf.PI *
            Mathf.Atan(SpawnerAsset.SpawnCooldownDifficultyModifier * DifficultyComponent.Difficulty);
    }
}