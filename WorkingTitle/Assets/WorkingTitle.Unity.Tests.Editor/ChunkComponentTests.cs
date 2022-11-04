using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorkingTitle.Lib.Pathfinding;
using WorkingTitle.Unity.Extensions;
using WorkingTitle.Unity.Map;

namespace WorkingTitle.Unity.Tests.Editor
{
    public class ChunkComponentTests
    {
        [Test]
        public void TestInitializeAfterMove()
        {
            var chunkObject = new GameObject("Chunk");
            chunkObject.SetActive(false);
            var chunkComponent = chunkObject.AddComponent<ChunkComponent>();
            
            var tile = ScriptableObject.CreateInstance<Tile>();
            var position = new Vector3Int(0, 0, 0);
            var size = new Vector3Int(10, 10, 1);
            var fromBounds = new BoundsInt(position, size);
            var toBounds = new BoundsInt(position + Vector3Int.up * size, size);
            
            var firstTilemapObject = new GameObject("FirstTilemap");
            var firstTilemap = firstTilemapObject.AddComponent<Tilemap>();
            
            var secondTilemapObject = new GameObject("SecondTilemap");
            var secondTilemap = secondTilemapObject.AddComponent<Tilemap>();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var cellPosition = new Vector3Int(x, y, 0);
                    firstTilemap.SetTile(cellPosition, tile);
                    secondTilemap.SetTile(cellPosition, tile);
                }
            }
            
            firstTilemapObject.transform.SetParent(chunkObject.transform);
            secondTilemapObject.transform.SetParent(chunkObject.transform);
            
            chunkComponent.Initialize();
            
            var expectedBounds = new BoundsInt(position, size);
            Assert.AreEqual(expectedBounds, chunkComponent.Bounds);

            foreach (var tilemap in chunkComponent.Tilemaps)
            {
                tilemap.MoveTiles(fromBounds, toBounds);
            }
        }
    }
}