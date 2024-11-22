using System;

namespace UnitySignals
{
    public class EffectDisposer : IDisposable
    {
        private readonly IReactive _reactive;

        public EffectDisposer(IReactive reactive)
        {
            _reactive = reactive;
        }

        public void Dispose()
        {
            _reactive.Effect = false;
        }
    }
}