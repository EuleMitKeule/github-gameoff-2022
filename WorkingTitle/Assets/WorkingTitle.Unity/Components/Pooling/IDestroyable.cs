using System;

namespace WorkingTitle.Unity.Components.Pooling
{
    public interface IDestroyable
    {
        public event EventHandler Destroyed;
    }
}