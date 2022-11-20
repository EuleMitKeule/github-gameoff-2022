using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using WorkingTitle.Unity.Assets.Graphics;
using WorkingTitle.Unity.Components.Pooling;

namespace WorkingTitle.Unity.Components.Graphics
{
    public class TrackSpawnerComponent : SerializedMonoBehaviour, IResettable
    {
        [OdinSerialize]
        TrackSpawnerAsset TrackSpawnerAsset { get; set; }
        
        [OdinSerialize]
        Transform TrackPointLeft { get; set; }
        
        [OdinSerialize]
        Transform TrackPointRight { get; set; }
        
        Vector2 LastTrackLeft { get; set; }
        Vector2 LastTrackRight { get; set; }
        
        TankComponent TankComponent { get; set; }
        PoolComponent PoolComponent { get; set; }
        
        void Awake()
        {
            TankComponent = GetComponent<TankComponent>();
            PoolComponent = FindObjectOfType<PoolComponent>();
        }
        
        void Update()
        {
            var trackDistanceLeft = ((Vector2)TrackPointLeft.position - LastTrackLeft).magnitude;
            var trackDistanceRight = ((Vector2)TrackPointRight.position - LastTrackRight).magnitude;

            if (trackDistanceLeft >= TrackSpawnerAsset.TrackDistance)
            {
                var position = TrackPointLeft.position;
                LastTrackLeft = position;
                
                SpawnTrack(position);
            }

            if (trackDistanceRight >= TrackSpawnerAsset.TrackDistance)
            {
                var position = TrackPointRight.position;
                LastTrackRight = position;
                
                SpawnTrack(position);
            }
        }
        
        public void Reset()
        {
            LastTrackLeft = TrackPointLeft.position;
            LastTrackRight = TrackPointRight.position;
        }

        void SpawnTrack(Vector2 position)
        {
            var trackObject = PoolComponent.Allocate(TrackSpawnerAsset.TrackPrefab);
            var trackComponent = trackObject.GetComponent<TrackComponent>();
            
            trackComponent.Initialize(position, TankComponent.TankBody.transform.rotation);
        }
    }
}