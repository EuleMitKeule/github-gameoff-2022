using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorkingTitle.Unity.Components.Map;
using WorkingTitle.Unity.Extensions;

namespace WorkingTitle.Unity.Assets
{
    [CreateAssetMenu(menuName = nameof(MapAsset), fileName = nameof(MapAsset))]
    public class MapAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public int ChunkSize { get; set; }
        
        [OdinSerialize]
        public int ViewDistance { get; set; }

        [OdinSerialize]
        [ValidateInput(nameof(IsAtLeastOneChunk), "There must be at least one chunk")]
        [ValidateInput(nameof(IsChunksSameSize), "All chunks must be the same size")]
        public List<GameObject> ChunkPrefabs { get; set; } = new();
        
        [OdinSerialize] public Dictionary<TilemapType, bool> TilemapIsWalkable { get; set; } = new();
        
        #region Editor

        bool IsAtLeastOneChunk => ChunkPrefabs.Count > 0;

        bool IsChunksSameSize => ChunkPrefabs
            .Select(chunkPrefab => chunkPrefab.GetComponentsInChildren<Tilemap>().GetBounds())
            .Distinct()
            .Count() == 1;
        
        #endregion
    }
}