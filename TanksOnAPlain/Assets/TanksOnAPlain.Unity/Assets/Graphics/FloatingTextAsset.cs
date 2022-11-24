using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TanksOnAPlain.Unity.Components.Graphics;
using UnityEngine;

namespace TanksOnAPlain.Unity.Assets.Graphics
{
    [CreateAssetMenu(menuName = nameof(FloatingTextAsset), fileName = nameof(FloatingTextAsset))]
    public class FloatingTextAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public float Speed { get; set; }
        
        [OdinSerialize]
        public float Duration { get; set; }
        
        [OdinSerialize]
        public Dictionary<TextColor, Color> TextColorToColor { get; private set; } = new()
        {
            {TextColor.White, new Color(1f, 1f, 1f)},
            {TextColor.Red, new Color(161f / 255f, 25f / 255f, 32f / 255f)},
            {TextColor.Purple, new Color(99f / 255f, 22f / 255f, 73f / 255f)},
            {TextColor.Blue, new Color(70f / 255f, 83f / 255f, 116f / 255f)},
            {TextColor.Green, new Color(85f / 255f, 99f / 255f, 46f / 255f)},
        };
    }
}