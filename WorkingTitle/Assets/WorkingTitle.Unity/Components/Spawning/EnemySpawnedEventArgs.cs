using UnityEngine;

namespace WorkingTitle.Unity.Components.Spawning
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