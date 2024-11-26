using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using static UnitySignals.Static;

namespace UnitySignals.Tests
{
    public class SchedulerTest
    {
        [UnityTest]
        public IEnumerator Auto_Stabilize()
        {
            var s = Signal(1);
            var newValue = 0;
            var oldValue = 0;

            var watcher = Watch(() => s.Value, (a, b) =>
            {
                newValue = a;
                oldValue = b;
            });

            s.Value = 5;
            yield return null;
            Assert.AreEqual(5, newValue);
            Assert.AreEqual(1, oldValue);

            s.Value = 8;
            yield return null;
            Assert.AreEqual(8, newValue);
            Assert.AreEqual(5, oldValue);

            watcher.Dispose();

            s.Value = 10;
            yield return null;
            Assert.AreEqual(8, newValue);
            Assert.AreEqual(5, oldValue);
        }
    }
}