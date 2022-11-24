using System;
using TanksOnAPlain.Unity.Assets.PowerUps;

namespace TanksOnAPlain.Unity.Components.PowerUps
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