using System.Collections.Generic;

namespace UnitySignals
{
    public class SharedState
    {
        public static readonly SharedState Instance = new();

        public IReactive CurrentReaction { get; set; }

        public List<IReactive> CurrentGets { get; set; }

        public int CurrentGetsIndex { get; set; }

        public List<IReactive> EffectQueue { get; } = new();

        public static void Stabilize()
        {
            foreach (var effectQueue in Instance.EffectQueue)
            {
                effectQueue.InvokeGet();
            }

            Instance.EffectQueue.Clear();
        }
    }
}