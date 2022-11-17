using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Components.Pooling;

namespace WorkingTitle.Unity.Components.Graphics
{
    public class TrackSpawnerComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        Transform TrackPointLeft { get; set; }
        
        [OdinSerialize]
        Transform TrackPointRight { get; set; }
        
        [OdinSerialize]
        float TrackDistance { get; set; }
        
        [OdinSerialize]
        GameObject TrackPrefab { get; set; }
        
        Vector2 LastTrackLeft { get; set; }
        Vector2 LastTrackRight { get; set; }
        
        TankComponent TankComponent { get; set; }
        PoolComponent PoolComponent { get; set; }
        
        void Awake()
        {
            TankComponent = GetComponent<TankComponent>();
            PoolComponent = FindObjectOfType<PoolComponent>();
            
            LastTrackLeft = TrackPointLeft.position;
            LastTrackRight = TrackPointRight.position;
        }
        
        void Update()
        {
            var trackDistanceLeft = ((Vector2)TrackPointLeft.position - LastTrackLeft).magnitude;
            var trackDistanceRight = ((Vector2)TrackPointRight.position - LastTrackRight).magnitude;

            if (trackDistanceLeft >= TrackDistance)
            {
                LastTrackLeft = TrackPointLeft.position;
                
                SpawnTrack(TrackPointLeft.position);
            }

            if (trackDistanceRight >= TrackDistance)
            {
                LastTrackRight = TrackPointRight.position;
                
                SpawnTrack(TrackPointRight.position);
            }
        }

        void SpawnTrack(Vector2 position)
        {
            var trackComponent = PoolComponent.Allocate<TrackComponent>(TrackPrefab);
            trackComponent.transform.position = position;
            trackComponent.transform.rotation = TankComponent.TankBody.transform.rotation;
        }
    }
}