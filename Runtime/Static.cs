using System;

namespace UnitySignals
{
    public static class Static
    {
        public static ISignal<T> Signal<T>(T value) where T : IEquatable<T>
        {
            return new Reactive<T>(value);
        }

        public static IComputed<T> Computed<T>(Func<T> func) where T : IEquatable<T>
        {
            return new Reactive<T>(func, false);
        }

        public static IDisposable Effect(Action action)
        {
            var effect = new Reactive<int>(() =>
            {
                action();
                return default;
            }, true);

            effect.Get();
            return new EffectDisposer(effect);
        }

        public static IDisposable Watch<T>(Func<T> func, Action<T, T> onChanged, bool immediate = false)
            where T : IEquatable<T>
        {
            var source = new Reactive<T>(func, false);
            var watcher = new Reactive<T>(() =>
            {
                source.OnCleanup(oldValue => onChanged(source.Value, oldValue));
                return source.Value;
            }, true);

            if (immediate) watcher.Get();
            return new EffectDisposer(watcher);
        }
    }
}