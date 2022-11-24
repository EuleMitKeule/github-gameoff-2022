using UnityEngine;
using UnityEngine.Tilemaps;

namespace TanksOnAPlain.Unity.Assets.Tiles
{
    [CreateAssetMenu(menuName = "Tile/" + nameof(PrefabTile), fileName = nameof(PrefabTile))]
    public class PrefabTile : TileBase
    {
        [SerializeField]
        Sprite tileSprite;
        
        [SerializeField]
        GameObject tileAssociatedPrefab;
        
        [SerializeField]
        float prefabLocalOffset = 0.5f;
        
        [SerializeField]
        float prefabZOffset = -1f;
        
        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject gameObject)
        {
            if (!gameObject) return true;
            
#if UNITY_EDITOR
            if (gameObject.scene.name == null)
            {
                DestroyImmediate(gameObject);
            }
#endif
            
            gameObject.transform.position = new Vector3(
                position.x + prefabLocalOffset, 
                position.y + prefabLocalOffset, 
                prefabZOffset);
            
            return true;
        }
        
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = Application.isPlaying ? null : tileSprite;
            
            if (tileAssociatedPrefab && !tileData.gameObject)
            {
                tileData.gameObject = tileAssociatedPrefab;
            }
        }
    }
}