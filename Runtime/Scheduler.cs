using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

namespace UnitySignals
{
    public class Scheduler
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var subSystemList = new List<PlayerLoopSystem>(currentPlayerLoop.subSystemList)
            {
                new()
                {
                    type = typeof(Scheduler),
                    updateDelegate = Update,
                }
            };

            currentPlayerLoop.subSystemList = subSystemList.ToArray();
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

        private static void Update()
        {
            SharedState.Stabilize();
        }
    }
}