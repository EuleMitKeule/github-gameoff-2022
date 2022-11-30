using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TanksOnAPlain.Unity.Components.Pooling;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Graphics
{
    public class ExplosionComponent : SerializedMonoBehaviour, IDestroyable, IResettable
    {
        List<ParticleSystem> ParticleSystems { get; } = new();
        
        float LongestDuration { get; set; }
        
        float DeathTime { get; set; }
        
        public event EventHandler Destroyed;

        void Awake()
        {
            var particleSystems = GetComponentsInChildren<ParticleSystem>();
            ParticleSystems.AddRange(particleSystems);
            
            foreach (var particleSystem in ParticleSystems)
            {
                var duration = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;
                if (duration > LongestDuration)
                {
                    LongestDuration = duration;
                }
            }
        }

        void Update()
        {
            if (Time.time >= DeathTime)
            {
                Destroyed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Reset()
        {
            DeathTime = Time.time + LongestDuration;
            
            foreach (var particleSystem in ParticleSystems)
            {
                particleSystem.Play();
            }
        }
    }
}