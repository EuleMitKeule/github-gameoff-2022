using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets.Graphics;
using WorkingTitle.Unity.Components.Health;
using WorkingTitle.Unity.Components.Pooling;

namespace WorkingTitle.Unity.Components.Graphics
{
    public class FloatingTextSpawnerComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        FloatingTextSpawnerAsset FloatingTextSpawnerAsset { get; set; }
        
        [OdinSerialize]
        Transform SpawnPoint { get; set; }
        
        PoolComponent PoolComponent { get; set; }
        HealthComponent HealthComponent { get; set; }

        void Start()
        {
            PoolComponent = FindObjectOfType<PoolComponent>();
            HealthComponent = GetComponent<HealthComponent>();
            
            HealthComponent.HealthChanged += OnHealthChanged;
        }

        void SpawnText(string text, TextColor textColor)
        {
            var floatingTextObject = PoolComponent.Allocate(FloatingTextSpawnerAsset.FloatingTextPrefab);
            var floatingTextComponent = floatingTextObject.GetComponent<FloatingTextComponent>();
            
            floatingTextComponent.Initialize(text, textColor, SpawnPoint.position);
        }

        void OnHealthChanged(object sender, HealthChangedEventArgs e)
        {
            var healthChange = Mathf.RoundToInt(e.HealthChange);
            var text = (healthChange > 0 ? "+" : "") + healthChange;
            var textColor = e.HealthChange > 0 ? TextColor.Green : TextColor.Red;
            
            SpawnText(text, textColor);
        }
    }
}