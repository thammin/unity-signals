using System;
using System.Collections.Generic;

namespace UnitySignals
{
    public interface IReactive
    {
        List<IReactive> Observers { get; set; }

        CacheState State { get; set; }

        bool Effect { get; set; }

        void InvokeGet();

        void Stale(CacheState state);

        void UpdateIfNecessary();
    }

    public interface IReactive<T> : IReactive where T : IEquatable<T>
    {
        T Value { get; set; }

        T Get();

        void Set(T Value);

        void Set(Func<T> func);

        void OnCleanup(Action<T> onCleanup);
    }
}