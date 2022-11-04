using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorkingTitle.Unity.Extensions;

namespace WorkingTitle.Unity.Map
{
    public class ChunkComponent : SerializedMonoBehaviour
    {
        [TitleGroup("Grid")]
        [OdinSerialize]
        [Required]
        [ValueDropdown(nameof(TilemapValues), IsUniqueList = true, ExcludeExistingValuesInList = true, DrawDropdownForListElements = false)]
        [ValidateInput(nameof(IsTilemapsUnique), "Tilemap can not be both walkable and obstacle.")]
        [ValidateInput(nameof(IsWalkableTilemapsNotEmpty), "At least one walkable tilemap is required.")]
        public List<Tilemap> WalkableTilemaps { get; private set; } = new ();
        
        [OdinSerialize]
        [Required]
        [ValueDropdown(nameof(TilemapValues), IsUniqueList = true, ExcludeExistingValuesInList = true, DrawDropdownForListElements = false)]
        [ValidateInput(nameof(IsTilemapsUnique), "Tilemap can not be both walkable and obstacle.")]
        public List<Tilemap> ObstacleTilemaps { get; private set; } = new ();
        
        public List<Tilemap> Tilemaps { get; private set; }
        
        public BoundsInt Bounds { get; private set; }
        
        #region Editor
        
        IEnumerable<Tilemap> TilemapValues => GetComponentsInChildren<Tilemap>();
        
        bool IsTilemapsUnique => !WalkableTilemaps.Any(e => ObstacleTilemaps.Contains(e));
        bool IsWalkableTilemapsNotEmpty => WalkableTilemaps.Any();
        
        #endregion

        void Awake()
        {
            Initialize();
        }
        
        public void Initialize(Vector3Int position = default)
        {
            Tilemaps = WalkableTilemaps.Concat(ObstacleTilemaps).ToList();

            foreach (var tilemap in Tilemaps)
            {
                tilemap.CompressBounds();
            }
            
            Bounds = Tilemaps.GetBounds(position);
        }
    }
}