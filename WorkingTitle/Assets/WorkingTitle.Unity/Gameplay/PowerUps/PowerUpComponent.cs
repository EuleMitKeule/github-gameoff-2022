using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    public class PowerUpComponent : SerializedMonoBehaviour
    {
        [OdinSerialize] public PowerUpAsset PowerUpAsset { get; set; }
    }
}