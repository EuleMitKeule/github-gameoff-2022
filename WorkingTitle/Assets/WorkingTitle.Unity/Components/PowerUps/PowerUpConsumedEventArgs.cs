using System;
using WorkingTitle.Unity.Assets.PowerUps;

namespace WorkingTitle.Unity.Components.PowerUps
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