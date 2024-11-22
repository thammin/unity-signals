using NUnit.Framework;
using static UnitySignals.Tests.Static;

namespace UnitySignals.Tests
{
    public class CoreTest
    {
        [Test]
        public void Two_Signals()
        {
            var a = Reactive(7);
            var b = Reactive(1);
            var callCount = 0;

            var c = Reactive(() =>
            {
                callCount++;
                return a.Value * b.Value;
            });

            a.Value = 2;
            Assert.AreEqual(2, c.Value);

            b.Value = 3;
            Assert.AreEqual(6, c.Value);

            Assert.AreEqual(2, callCount);
            c.Get();
            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void Dependent_Computed()
        {
            var a = Reactive(7);
            var b = Reactive(1);
            var callCount1 = 0;
            var c = Reactive(() =>
            {
                callCount1++;
                return a.Value * b.Value;
            });

            var callCount2 = 0;
            var d = Reactive(() =>
            {
                callCount2++;
                return c.Value + 1;
            });

            Assert.AreEqual(8, d.Value);
            Assert.AreEqual(1, callCount1);
            Assert.AreEqual(1, callCount2);
            a.Value = 3;
            Assert.AreEqual(4, d.Value);
            Assert.AreEqual(2, callCount1);
            Assert.AreEqual(2, callCount2);
        }

        [Test]
        public void Equality_Check()
        {
            var callCount = 0;
            var a = Reactive(7);
            var c = Reactive(() =>
            {
                callCount++;
                return a.Value * 10;
            });
            c.Get();
            c.Get();
            Assert.AreEqual(1, callCount);
            a.Value = 7;
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void Dynamic_Computed()
        {
            var a = Reactive(1);
            var b = Reactive(2);
            var callCountA = 0;
            var callCountB = 0;
            var callCountAB = 0;

            var cA = Reactive(() =>
            {
                callCountA++;
                return a.Value;
            });

            var cB = Reactive(() =>
            {
                callCountB++;
                return b.Value;
            });

            var cAB = Reactive(() =>
            {
                callCountAB++;

                var v = cA.Value;
                if (v == 0)
                {
                    return cB.Value;
                }

                return v;
            });

            Assert.AreEqual(1, cAB.Value);
            a.Value = 2;
            b.Value = 3;
            Assert.AreEqual(2, cAB.Value);

            Assert.AreEqual(2, callCountA);
            Assert.AreEqual(2, callCountAB);
            Assert.AreEqual(0, callCountB);
            a.Value = 0;
            Assert.AreEqual(3, cAB.Value);
            Assert.AreEqual(3, callCountA);
            Assert.AreEqual(3, callCountAB);
            Assert.AreEqual(1, callCountB);
            b.Value = 4;
            Assert.AreEqual(4, cAB.Value);
            Assert.AreEqual(3, callCountA);
            Assert.AreEqual(4, callCountAB);
            Assert.AreEqual(2, callCountB);
        }

        [Test]
        public void Boolean_Equality_Check()
        {
            var a = Reactive(0);
            var b = Reactive(() => a.Value > 0);
            var callCount = 0;
            var c = Reactive(() =>
            {
                callCount++;
                return b.Value ? 1 : 0;
            });

            Assert.AreEqual(0, c.Value);
            Assert.AreEqual(1, callCount);

            a.Value = 1;
            Assert.AreEqual(1, c.Value);
            Assert.AreEqual(2, callCount);

            a.Value = 2;
            Assert.AreEqual(1, c.Value);
            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void Diamond_Computeds()
        {
            var s = Reactive(1);
            var a = Reactive(s.Get);
            var b = Reactive(() => a.Value * 2);
            var c = Reactive(() => a.Value * 3);
            var callCount = 0;
            var d = Reactive(() =>
            {
                callCount++;
                return b.Value + c.Value;
            });
            Assert.AreEqual(5, d.Value);
            Assert.AreEqual(1, callCount);
            s.Value = 2;
            Assert.AreEqual(10, d.Value);
            Assert.AreEqual(2, callCount);
            s.Value = 3;
            Assert.AreEqual(15, d.Value);
            Assert.AreEqual(3, callCount);
        }

        [Test]
        public void Set_Inside_Reaction()
        {
            var s = Reactive(1);
            var a = Reactive(() => s.Value = 2);
            var l = Reactive(() => s.Value + 100);

            a.Get();
            Assert.AreEqual(102, l.Value);
        }

        [Test]
        public void Cleanup()
        {
            var s = Reactive(1);
            var oldS = 0;

            var l = Reactive(() =>
            {
                s.OnCleanup(x => oldS = x);
                return s.Value + 1;
            });

            l.Get();
            Assert.AreEqual(0, oldS);

            s.Value = 5;
            l.Get();
            Assert.AreEqual(2, oldS);

            s.Value = 10;
            l.Get();
            Assert.AreEqual(6, oldS);
        }
    }
}