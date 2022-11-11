using System;

namespace WorkingTitle.Unity.Gameplay.PowerUps
{
    public class PowerUpConsumedEventArgs : EventArgs
    {
        public PowerUpAsset PowerUpAsset { get; }

        public PowerUpConsumedEventArgs(PowerUpAsset powerUpAsset)
        {
            PowerUpAsset = powerUpAsset;
        }
    }
}