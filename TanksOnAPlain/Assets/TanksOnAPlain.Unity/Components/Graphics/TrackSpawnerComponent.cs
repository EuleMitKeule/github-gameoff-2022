using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Assets.Graphics;
using TanksOnAPlain.Unity.Components.Pooling;
using UnityEngine;

namespace TanksOnAPlain.Unity.Components.Graphics
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
        
        Vector2 LastPositionLeft { get; set; }
        Vector2 LastPositionRight { get; set; }
        
        Animator AnimatorTrackLeft { get; set; }
        Animator AnimatorTrackRight { get; set; }
        
        static readonly int forwardsHash = Animator.StringToHash("forwards");
        
        TankComponent TankComponent { get; set; }
        PoolComponent PoolComponent { get; set; }
        
        void Awake()
        {
            TankComponent = GetComponent<TankComponent>();
            PoolComponent = FindObjectOfType<PoolComponent>();
            
            var animators = GetComponentsInChildren<Animator>();
            AnimatorTrackLeft = animators[0];
            AnimatorTrackRight = animators[1];
        }
        
        void FixedUpdate()
        {
            var leftPosition = (Vector2)TrackPointLeft.position;
            var rightPosition = (Vector2)TrackPointRight.position;
            
            var lastDirectionLeft = leftPosition - LastPositionLeft;
            var lastDirectionRight = rightPosition - LastPositionRight;
            
            var leftDistanceSign = Mathf.Sign(Vector2.Dot(lastDirectionLeft, TankComponent.TankBody.transform.up));
            var rightDistanceSign = Mathf.Sign(Vector2.Dot(lastDirectionRight, TankComponent.TankBody.transform.up));
            
            var frameDistanceLeft = lastDirectionLeft.magnitude;
            var frameDistanceRight = lastDirectionRight.magnitude;
            
            AnimatorTrackLeft.SetBool(forwardsHash, leftDistanceSign >= 0);
            AnimatorTrackRight.SetBool(forwardsHash, rightDistanceSign >= 0);

            AnimatorTrackLeft.speed = frameDistanceLeft / Time.fixedDeltaTime;
            AnimatorTrackRight.speed = frameDistanceRight / Time.fixedDeltaTime;
            
            var trackDistanceLeft = (leftPosition - LastTrackLeft).magnitude;
            var trackDistanceRight = (rightPosition - LastTrackRight).magnitude;

            if (trackDistanceLeft >= TrackSpawnerAsset.TrackDistance)
            {
                LastTrackLeft = leftPosition;
                
                SpawnTrack(leftPosition);
            }

            if (trackDistanceRight >= TrackSpawnerAsset.TrackDistance)
            {
                LastTrackRight = rightPosition;
                
                SpawnTrack(rightPosition);
            }
            
            LastPositionLeft = leftPosition;
            LastPositionRight = rightPosition;
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