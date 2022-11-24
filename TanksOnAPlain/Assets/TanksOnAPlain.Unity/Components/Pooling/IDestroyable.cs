using System;

namespace TanksOnAPlain.Unity.Components.Pooling
{
    public interface IDestroyable
    {
        public event EventHandler Destroyed;
    }
}