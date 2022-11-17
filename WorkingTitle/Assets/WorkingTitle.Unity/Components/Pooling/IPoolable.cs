using System;

namespace WorkingTitle.Unity.Components.Pooling
{
    public interface IPoolable
    {
        public event EventHandler Destroyed;

        public void Reset();
    }
}