using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorkingTitle.Unity.Extensions;

namespace WorkingTitle.Unity.Components.Map
{
    public class ChunkComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Required]
        public Dictionary<TilemapType, Tilemap> Tilemaps { get; set; } = new();

        public BoundsInt Bounds { get; private set; }
        
        void Awake()
        {
            Initialize();
        }
        
        public void Initialize(Vector3Int position = default)
        {
            foreach (var tilemap in Tilemaps.Values)
            {
                tilemap.CompressBounds();
            }
            
            Bounds = Tilemaps.Values.GetBounds(position);
        }
    }
}