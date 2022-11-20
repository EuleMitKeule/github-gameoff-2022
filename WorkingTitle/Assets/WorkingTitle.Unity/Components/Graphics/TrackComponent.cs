using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets.Graphics;
using WorkingTitle.Unity.Components.Pooling;

namespace WorkingTitle.Unity.Components.Graphics
{
    public class TrackComponent : SerializedMonoBehaviour, IResettable, IDestroyable
    {
        [OdinSerialize]
        TrackAsset TrackAsset { get; set; }
        
        float StartTime { get; set; }
        
        SpriteRenderer SpriteRenderer { get; set; }

        public event EventHandler Destroyed;

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

        public void Initialize(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
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