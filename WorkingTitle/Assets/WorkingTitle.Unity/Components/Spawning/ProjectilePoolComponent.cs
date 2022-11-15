using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WorkingTitle.Unity.Assets;
using WorkingTitle.Unity.Components.Physics;

namespace WorkingTitle.Unity.Components.Spawning
{
    public class ProjectilePoolComponent : SerializedMonoBehaviour
    {
        Dictionary<GameObject, Queue<ProjectileComponent>> AvailableProjectilesPerPrefab { get; set; } = new();
        
        Dictionary<GameObject, GameObject> PrefabToPool { get; set; } = new();
        
        GameObject ProjectilePrefab { get; set; }

        public ProjectileComponent AllocateProjectile(GameObject projectilePrefab)
        {
            if (!AvailableProjectilesPerPrefab.ContainsKey(projectilePrefab))
            {
                AvailableProjectilesPerPrefab[projectilePrefab] = new Queue<ProjectileComponent>();

                var poolObject = new GameObject($"pool_{projectilePrefab.name}");
                poolObject.transform.SetParent(transform);
                
                PrefabToPool[projectilePrefab] = poolObject;
            }

            var projectileComponent = GetOrCreateProjectile(projectilePrefab);

            return projectileComponent;
        }

        ProjectileComponent GetOrCreateProjectile(GameObject projectilePrefab)
        {
            var availableProjectiles = AvailableProjectilesPerPrefab[projectilePrefab];
            if (availableProjectiles.Count > 0)
            {
                var dequeuedProjectileComponent = availableProjectiles.Dequeue();
                dequeuedProjectileComponent.gameObject.SetActive(true);
                
                return dequeuedProjectileComponent;
            }
            
            var projectileObject = Instantiate(projectilePrefab, transform);
            var poolObject = PrefabToPool[projectilePrefab];
            projectileObject.transform.SetParent(poolObject.transform);

            var projectileComponent = projectileObject.GetComponent<ProjectileComponent>();
            projectileComponent.Destroyed += (sender, e) => OnProjectileDestroyed(sender, e, projectilePrefab);
            
            return projectileComponent;
        }

        void OnProjectileDestroyed(object sender, EventArgs e, GameObject projectilePrefab)
        {
            var projectileComponent = (ProjectileComponent) sender;
            
            projectileComponent.gameObject.SetActive(false);
            
            AvailableProjectilesPerPrefab[projectilePrefab].Enqueue(projectileComponent);
        }
    }
}