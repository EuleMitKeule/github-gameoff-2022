using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using WorkingTitle.Unity.Assets.Graphics;
using WorkingTitle.Unity.Components.Pooling;

namespace WorkingTitle.Unity.Components.Graphics
{
    public class FloatingTextComponent : SerializedMonoBehaviour, IPoolable
    {
        [OdinSerialize]
        Dictionary<TextColor, Color> TextColorToColor = new Dictionary<TextColor, Color>()
        {
            {TextColor.White, new Color(1f, 1f, 1f)},
            {TextColor.Red, new Color(161f / 255f, 25f / 255f, 32f / 255f)},
            {TextColor.Purple, new Color(99f / 255f, 22f / 255f, 73f / 255f)},
            {TextColor.Blue, new Color(70f / 255f, 83f / 255f, 116f / 255f)},
            {TextColor.Green, new Color(85f / 255f, 99f / 255f, 46f / 255f)},
        };
        
        [OdinSerialize]
        FloatingTextAsset FloatingTextAsset { get; set; }
        
        public float Speed { get; set; }
        
        public float Duration { get; set; }

        Color Color { get; set; }

        float StartTime { get; set; }

        public event EventHandler Destroyed;
        
        TextMeshProUGUI TextMesh { get; set; }

        void Awake()
        {
            TextMesh = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Initialize(string text, TextColor textColor, Vector3 position)
        {
            TextMesh.SetText(text);
            TextMesh.color = TextColorToColor[textColor];
            
            transform.position = position;
        }

        public void Reset()
        {
            Speed = FloatingTextAsset.Speed;
            Duration = FloatingTextAsset.Duration;
            StartTime = Time.time;
        }

        void Update()
        {
            var aliveTime = Time.time - StartTime;
            var lifeTime = aliveTime / Duration;

            if (lifeTime >= 1f)
            {
                Destroyed?.Invoke(this, EventArgs.Empty);
            }
            
            var alpha = Mathf.Lerp(1f, 0f, lifeTime);
            var color = TextMesh.color;
            color = new Color(color.r, color.g, color.b, alpha);
            TextMesh.color = color;

            var newPosition = transform.position + Vector3.up * (Speed * Time.deltaTime);
            transform.position = newPosition;
        }
    }
}