using NUnit.Framework;
using static UnitySignals.Tests.Static;

namespace UnitySignals.Tests
{
    public class DynamicTest
    {
        [Test]
        public void Dynamic_Sources_Recalculate_Correctly()
        {
            var a = Reactive(false);
            var b = Reactive(2);
            var count = 0;

            var c = Reactive(() =>
            {
                count++;
                if (a.Value)
                {
                    b.Get();
                }

                return 0;
            });

            c.Get();
            Assert.AreEqual(1, count);
            a.Value = true;
            c.Get();
            Assert.AreEqual(2, count);

            b.Value = 4;
            c.Get();
            Assert.AreEqual(3, count);

            c.Get();
            Assert.AreEqual(3, count);
        }

        [Test]
        public void Dynamic_Sources_Dont_Reexecute_A_Parent_Unnecessarily()
        {
            var s = Reactive(2);
            var a = Reactive(() => s.Value + 1);
            var bCount = 0;
            var b = Reactive(() =>
            {
                bCount++;
                return s.Value + 10;
            });
            var l = Reactive(() =>
            {
                var result = a.Value;
                if ((result & 0x1) != 0)
                {
                    result += b.Value;
                }

                return result;
            });

            Assert.AreEqual(15, l.Value);
            Assert.AreEqual(1, bCount);
            s.Value = 3;
            Assert.AreEqual(4, l.Value);
            Assert.AreEqual(1, bCount);
        }
    }
}