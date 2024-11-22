using System;
using System.Collections.Generic;

namespace UnitySignals
{
    public class Reactive<T> : IReactive<T>, ISignal<T>, IComputed<T>
        where T : IEquatable<T>
    {
        private T _value;
        private Func<T> _func;
        private List<IReactive> _sources;

        public List<IReactive> Observers { get; set; }
        public CacheState State { get; set; }
        public List<Action<T>> Cleanups { get; } = new();
        public bool Effect { get; set; }

        public T Value
        {
            get => Get();
            set => Set(value);
        }

        public Reactive(T value)
        {
            _value = value;
            State = CacheState.Clean;
        }

        public Reactive(Func<T> func, bool effect)
        {
            _func = func;
            Effect = effect;
            State = CacheState.Dirty;

            if (effect)
            {
                SharedState.Instance.EffectQueue.Add(this);
            }
        }

        public T Get()
        {
            if (SharedState.Instance.CurrentReaction != null)
            {
                if (SharedState.Instance.CurrentGets == null &&
                    SharedState.Instance.CurrentReaction.Observers != null &&
                    SharedState.Instance.CurrentReaction.Observers[SharedState.Instance.CurrentGetsIndex] == this)
                {
                    SharedState.Instance.CurrentGetsIndex++;
                }
                else
                {
                    if (SharedState.Instance.CurrentGets == null)
                    {
                        SharedState.Instance.CurrentGets = new List<IReactive> { this };
                    }
                    else
                    {
                        SharedState.Instance.CurrentGets.Add(this);
                    }
                }
            }

            if (_func != null) UpdateIfNecessary();

            return _value;
        }

        public void Set(T value)
        {
            if (_func != null)
            {
                RemoveParentObservers(0);
                _sources = null;
                _func = null;
            }

            if (_value.Equals(value) == false)
            {
                if (Observers != null)
                {
                    foreach (var observer in Observers)
                    {
                        observer.Stale(CacheState.Dirty);
                    }
                }

                _value = value;
            }
        }

        public void Set(Func<T> func)
        {
            if (_func != func)
            {
                Stale(CacheState.Dirty);
            }

            _func = func;
        }

        public void Stale(CacheState state)
        {
            if (State < state)
            {
                if (State == CacheState.Clean && Effect)
                {
                    SharedState.Instance.EffectQueue.Add(this);
                }

                State = state;
                if (Observers != null)
                {
                    foreach (var observer in Observers)
                    {
                        observer.Stale(CacheState.Check);
                    }
                }
            }
        }

        private void Update()
        {
            var oldValue = _value;

            var prevReaction = SharedState.Instance.CurrentReaction;
            var prevGets = SharedState.Instance.CurrentGets;
            var prevIndex = SharedState.Instance.CurrentGetsIndex;

            SharedState.Instance.CurrentReaction = this;
            SharedState.Instance.CurrentGets = null;
            SharedState.Instance.CurrentGetsIndex = 0;

            try
            {
                foreach (var cleanup in Cleanups)
                {
                    cleanup.Invoke(_value);
                }

                Cleanups.Clear();

                if (_func != null)
                {
                    _value = _func.Invoke();
                }

                if (SharedState.Instance.CurrentGets != null)
                {
                    RemoveParentObservers(SharedState.Instance.CurrentGetsIndex);

                    if (_sources != null && SharedState.Instance.CurrentGetsIndex > 0)
                    {
                        foreach (var currentGet in SharedState.Instance.CurrentGets)
                        {
                            _sources.Add(currentGet);
                        }
                    }
                    else
                    {
                        _sources = SharedState.Instance.CurrentGets;
                    }

                    for (var i = SharedState.Instance.CurrentGetsIndex; i < _sources.Count; i++)
                    {
                        var source = _sources[i];
                        if (source.Observers == null)
                        {
                            source.Observers = new List<IReactive> { this };
                        }
                        else
                        {
                            source.Observers.Add(this);
                        }
                    }
                }
                else if (_sources != null && SharedState.Instance.CurrentGetsIndex < _sources.Count)
                {
                    RemoveParentObservers(SharedState.Instance.CurrentGetsIndex);
                    _sources.RemoveRange(SharedState.Instance.CurrentGetsIndex,
                        _sources.Count - SharedState.Instance.CurrentGetsIndex);
                }
            }
            finally
            {
                SharedState.Instance.CurrentGets = prevGets;
                SharedState.Instance.CurrentReaction = prevReaction;
                SharedState.Instance.CurrentGetsIndex = prevIndex;
            }

            if (oldValue.Equals(_value) == false && Observers != null)
            {
                foreach (var observer in Observers)
                {
                    observer.State = CacheState.Dirty;
                }
            }

            State = CacheState.Clean;
        }

        public void UpdateIfNecessary()
        {
            if (State == CacheState.Check)
            {
                if (_sources != null)
                {
                    foreach (var source in _sources)
                    {
                        source.UpdateIfNecessary();

                        if (State == CacheState.Dirty)
                        {
                            break;
                        }
                    }
                }
            }

            if (State == CacheState.Dirty)
            {
                Update();
            }

            State = CacheState.Clean;
        }

        private void RemoveParentObservers(int index)
        {
            if (_sources == null) return;

            for (var i = index; i < _sources.Count; i++)
            {
                var source = _sources[i];
                if (source.Observers != null)
                {
                    var swap = source.Observers.FindIndex(x => x == this);
                    source.Observers[swap] = source.Observers[^1];
                    source.Observers.RemoveAt(source.Observers.Count - 1);
                }
            }
        }

        public void OnCleanup(Action<T> onCleanup)
        {
            if (SharedState.Instance.CurrentReaction is Reactive<T> currentReaction)
            {
                currentReaction.Cleanups.Add(onCleanup);
            }
            else
            {
                throw new Exception("OnCleanup must be called from within a reactive function");
            }
        }

        void IReactive.InvokeGet() => Get();
    }
}