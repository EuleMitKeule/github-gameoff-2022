using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets.Graphics;
using WorkingTitle.Unity.Components.Pooling;

namespace WorkingTitle.Unity.Components.Graphics
{
    public class TrackComponent : SerializedMonoBehaviour, IPoolable
    {
        [OdinSerialize]
        TrackAsset TrackAsset { get; set; }
        
        float StartTime { get; set; }

        public event EventHandler Destroyed;
        
        SpriteRenderer SpriteRenderer { get; set; }

        void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            var lifeTime = Time.time - StartTime;
            if (lifeTime >= TrackAsset.LifeTime)
            {
                Destroyed?.Invoke(this, EventArgs.Empty);
            }
                
            var alpha = Mathf.Lerp(TrackAsset.StartAlpha, 0f, lifeTime  / TrackAsset.LifeTime);
            var color = SpriteRenderer.color;
            var newColor = new Color(color.r, color.g, color.b, alpha);
            
            SpriteRenderer.color = newColor;
        }

        public void Reset()
        {
            var color = SpriteRenderer.color;
            var newColor = new Color(color.r, color.g, color.b, TrackAsset.StartAlpha);
            
            SpriteRenderer.color = newColor;
            StartTime = Time.time;
        }
    }
}