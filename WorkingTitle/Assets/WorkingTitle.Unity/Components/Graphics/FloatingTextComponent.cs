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
    public class FloatingTextComponent : SerializedMonoBehaviour, IResettable, IDestroyable
    {
        [OdinSerialize]
        FloatingTextAsset FloatingTextAsset { get; set; }
        
        float Speed { get; set; }
        float Duration { get; set; }
        float StartTime { get; set; }

        TextMeshProUGUI TextMesh { get; set; }

        public event EventHandler Destroyed;

        void Awake()
        {
            TextMesh = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Initialize(string text, TextColor textColor, Vector3 position)
        {
            TextMesh.SetText(text);
            TextMesh.color = FloatingTextAsset.TextColorToColor[textColor];
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