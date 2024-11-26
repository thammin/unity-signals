using NUnit.Framework;
using static UnitySignals.Static;

namespace UnitySignals.Tests
{
    public class StaticTest
    {
        [Test]
        public void Create_Effect()
        {
            var s = Signal(1);
            var count = 0;
            var effect = Effect(() => count = s.Value + 1);

            Assert.AreEqual(2, count);

            s.Value = 4;
            SharedState.Stabilize();
            Assert.AreEqual(5, count);

            effect.Dispose();

            s.Value = 0;
            SharedState.Stabilize();
            Assert.AreEqual(5, count);
        }

        [Test]
        public void Create_Watcher()
        {
            var s = Signal(1);
            var newValue = 0;
            var oldValue = 0;

            var watcher = Watch(() => s.Value, (a, b) =>
            {
                newValue = a;
                oldValue = b;
            });

            SharedState.Stabilize();

            s.Value = 5;
            SharedState.Stabilize();
            Assert.AreEqual(5, newValue);
            Assert.AreEqual(1, oldValue);

            s.Value = 8;
            SharedState.Stabilize();
            Assert.AreEqual(8, newValue);
            Assert.AreEqual(5, oldValue);

            watcher.Dispose();

            s.Value = 10;
            SharedState.Stabilize();
            Assert.AreEqual(8, newValue);
            Assert.AreEqual(5, oldValue);
        }

        [Test]
        public void Create_Watcher_Immediate()
        {
            var s = Signal(1);
            var newValue = 0;
            var oldValue = 0;

            var watcher = Watch(() => s.Value, (a, b) =>
            {
                newValue = a;
                oldValue = b;
            }, true);

            s.Value = 5;
            SharedState.Stabilize();
            Assert.AreEqual(5, newValue);
            Assert.AreEqual(1, oldValue);

            s.Value = 8;
            SharedState.Stabilize();
            Assert.AreEqual(8, newValue);
            Assert.AreEqual(5, oldValue);

            watcher.Dispose();

            s.Value = 10;
            SharedState.Stabilize();
            Assert.AreEqual(8, newValue);
            Assert.AreEqual(5, oldValue);
        }
    }
}