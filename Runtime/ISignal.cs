using System;

namespace UnitySignals
{
    public interface ISignal<T> where T : IEquatable<T>
    {
        T Value { get; set; }
    }
}