using System;

namespace UnitySignals
{
    public interface IComputed<out T> where T : IEquatable<T>
    {
        T Value { get; }
    }
}