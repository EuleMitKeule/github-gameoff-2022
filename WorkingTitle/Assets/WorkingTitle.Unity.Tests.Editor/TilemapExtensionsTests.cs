using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using WorkingTitle.Unity.Extensions;

namespace WorkingTitle.Unity.Tests.Editor
{
    public class TilemapExtensionsTests
    {
        [Test]
        public void TestMoveTiles()
        {
            var gameObject = new GameObject();
            var tilemap = gameObject.AddComponent<Tilemap>();
            var tile = ScriptableObject.CreateInstance<Tile>();
            var size = new Vector3Int(10, 10, 1);
            
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var position = new Vector3Int(x, y, 0);
                    tilemap.SetTile(position, tile);
                }
            }

            var fromBounds = tilemap.cellBounds;

            var toBounds = new BoundsInt
            {
                xMin = fromBounds.xMax,
                xMax = fromBounds.xMax + fromBounds.size.x,
                yMin = fromBounds.yMin,
                yMax = fromBounds.yMax,
            };
            
            tilemap.MoveTiles(fromBounds, toBounds);
            
            var nullTiles = tilemap.GetTilesBlock(fromBounds);
            var movedTiles = tilemap.GetTilesBlock(toBounds);
            
            Assert.That(nullTiles, Is.All.Null);
            Assert.That(movedTiles, Is.All.Not.Null);
        }

        [Test]
        public void TestGetBounds()
        {
            var firstGameObject = new GameObject();
            var firstTilemap = firstGameObject.AddComponent<Tilemap>();
            var secondGameObject = new GameObject();
            var secondTilemap = secondGameObject.AddComponent<Tilemap>();
            var tile = ScriptableObject.CreateInstance<Tile>();
            var size = new Vector3Int(10, 10, 1);
            
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    var position = new Vector3Int(x, y, 0);
                    firstTilemap.SetTile(position, tile);
                    secondTilemap.SetTile(position, tile);
                }
            }
            
            var tilemaps = new[] {firstTilemap, secondTilemap};
            var bounds = tilemaps.GetBounds();
            
            Assert.That(bounds, Is.EqualTo(firstTilemap.cellBounds));
        }
    }
}