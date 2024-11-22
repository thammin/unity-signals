using NUnit.Framework;
using static UnitySignals.Tests.Static;

namespace UnitySignals.Tests
{
    public class ReactiveTest
    {
        [Test]
        public void Setting_A_Memo_To_Different_Memo()
        {
            var a = Reactive(1);
            var b = Reactive(() => a.Value * 2);
            var c = Reactive(() => b.Value);

            Assert.AreEqual(2, c.Value);
            b.Set(() => a.Value * 3);
            Assert.AreEqual(3, c.Value);
        }

        [Test]
        public void Setting_A_Memo_To_A_Signal()
        {
            var a = Reactive(1);
            var b = Reactive(() => a.Value * 2);
            var c = Reactive(() => b.Value);

            Assert.AreEqual(2, c.Value);
            b.Set(8);
            Assert.AreEqual(8, c.Value);

            a.Set(0);
            Assert.AreEqual(8, c.Value);
        }

        [Test]
        public void Effect_Run_On_Stablilize()
        {
            var src = Reactive(7);
            var effectCount = 0;

            var b = Reactive(() =>
            {
                effectCount++;
                return src.Value;
            }, true);

            Assert.AreEqual(7, b.Value);
            Assert.AreEqual(1, effectCount);

            src.Value = 6;
            SharedState.Stabilize();
            Assert.AreEqual(2, effectCount);

            Assert.AreEqual(6, b.Value);
            Assert.AreEqual(2, effectCount);
        }
    }
}