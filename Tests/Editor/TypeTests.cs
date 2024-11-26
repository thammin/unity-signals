using System;
using NUnit.Framework;
using static UnitySignals.Tests.Static;

namespace UnitySignals.Tests
{
    public class TypeTests
    {
        [Test]
        public void Type_String()
        {
            var lastName = Reactive("Young");
            var firstName = Reactive("Paul");
            var callCount = 0;

            var fullName = Reactive(() =>
            {
                callCount++;
                return firstName.Value + lastName.Value;
            });

            Assert.AreEqual(0, callCount);

            Assert.AreEqual("PaulYoung", fullName.Value);
            Assert.AreEqual(1, callCount);

            Assert.AreEqual("PaulYoung", fullName.Value);
            Assert.AreEqual(1, callCount);
        }

        public class TestObject : IEquatable<TestObject>
        {
            public int Number { get; set; }

            public bool Equals(TestObject other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Number == other.Number;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TestObject)obj);
            }

            public override int GetHashCode()
            {
                return Number;
            }

            public static bool operator ==(TestObject left, TestObject right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TestObject left, TestObject right)
            {
                return !Equals(left, right);
            }
        }

        [Test]
        public void Type_Object()
        {
            var a = new TestObject() { Number = 1 };
            var b = new TestObject() { Number = 1 };
            var c = new TestObject() { Number = 2 };
            var callCount = 0;

            var source = Reactive(a);
            var computed = Reactive(() =>
            {
                callCount++;
                return source.Value;
            });

            Assert.AreEqual(0, callCount);

            Assert.AreEqual(1, computed.Value.Number);
            Assert.AreEqual(1, callCount);

            source.Value = b;
            Assert.AreEqual(1, computed.Value.Number);
            Assert.AreEqual(1, callCount);

            source.Value = c;
            Assert.AreEqual(2, computed.Value.Number);
            Assert.AreEqual(2, callCount);
        }

        public record TestRecord(int number, string text);

        [Test]
        public void Type_Record()
        {
            var a = new TestRecord(1, "a");
            var b = new TestRecord(1, "a");
            var c = new TestRecord(1, "c");

            var callCount = 0;

            var source = Reactive(a);
            var computed = Reactive(() =>
            {
                callCount++;
                return source.Value;
            });

            Assert.AreEqual(0, callCount);

            Assert.AreEqual(1, computed.Value.number);
            Assert.AreEqual("a", computed.Value.text);
            Assert.AreEqual(1, callCount);

            source.Value = b;
            Assert.AreEqual(1, computed.Value.number);
            Assert.AreEqual("a", computed.Value.text);
            Assert.AreEqual(1, callCount);

            source.Value = c;
            Assert.AreEqual(1, computed.Value.number);
            Assert.AreEqual("c", computed.Value.text);
            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void Type_Tuple()
        {
            var a = (1, "a");
            var b = (1, "a");
            var c = (1, "c");

            var callCount = 0;

            var source = Reactive(a);
            var computed = Reactive(() =>
            {
                callCount++;
                return source.Value;
            });

            Assert.AreEqual(0, callCount);

            Assert.AreEqual(1, computed.Value.Item1);
            Assert.AreEqual("a", computed.Value.Item2);
            Assert.AreEqual(1, callCount);

            source.Value = b;
            Assert.AreEqual(1, computed.Value.Item1);
            Assert.AreEqual("a", computed.Value.Item2);
            Assert.AreEqual(1, callCount);

            source.Value = c;
            Assert.AreEqual(1, computed.Value.Item1);
            Assert.AreEqual("c", computed.Value.Item2);
            Assert.AreEqual(2, callCount);
        }
    }
}