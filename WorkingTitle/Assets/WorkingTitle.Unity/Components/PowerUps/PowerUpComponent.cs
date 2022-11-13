using Sirenix.OdinInspector;
using Sirenix.Serialization;
using WorkingTitle.Unity.Assets.PowerUps;

namespace WorkingTitle.Unity.Components.PowerUps
{
    public class PowerUpComponent : SerializedMonoBehaviour
    {
        [OdinSerialize] public PowerUpAsset PowerUpAsset { get; set; }
    }
}