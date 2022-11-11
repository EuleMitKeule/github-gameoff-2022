using UnityEngine;

namespace WorkingTitle.Unity.Gameplay.Spawning
{
    public class EnemySpawnedEventArgs
    {
        public GameObject Enemy { get; }
        
        public EnemySpawnedEventArgs(GameObject enemy)
        {
            Enemy = enemy;
        }
    }
}