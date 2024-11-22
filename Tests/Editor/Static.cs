using System;

namespace UnitySignals.Tests
{
    public static class Static
    {
        public static IReactive<T> Reactive<T>(T value) where T : IEquatable<T>
        {
            return new Reactive<T>(value);
        }

        public static IReactive<T> Reactive<T>(Func<T> func, bool effect = false) where T : IEquatable<T>
        {
            return new Reactive<T>(func, effect);
        }
    }
}