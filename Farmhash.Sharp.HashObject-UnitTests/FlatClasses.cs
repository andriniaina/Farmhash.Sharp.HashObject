using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Farmhash.Sharp;
using System.Linq;

namespace Farmhash.Sharp.UnitTests
{
    [TestClass]
    public class FlatClasses
    {
    class MyClass
    {
        public int P1 { get; set; }
        public int P2 { get; set; }
    }

    class MyClass2
    {
        public int P1 { get; set; }
        public int P2 { get; set; }
        public long P3 { get; set; }
    }
    class MyClass3
    {
        public int P1 { get; set; }
        public long P3 { get; set; }
        public string P4 { get; set; }
    }
    class MyClass4
    {
        public Boolean P1 { get; set; } = true;
        public Byte P2 { get; set; } = 5;
        public SByte P3 { get; set; } = -4;
        public Int16 P4 { get; set; } = -1254;
        public UInt16 P5 { get; set; } = 654;
        public Int32 P6 { get; set; } = -32165487;
        public UInt32 P7 { get; set; } = 5321657;
        public Int64 P8 { get; set; } = -321548254987;
        public UInt64 P9 { get; set; } = 984654312678;
        public Char P12 { get; set; } = 'r';
        public Double P13 { get; set; } = 2154.325876553352;
        public Single P14 { get; set; } = 1549674.32005746106F;
    }

        [TestMethod]
        public void Verify()
        {
            Assert.AreEqual(14924216523195090866UL, HashObject.Hash64(new MyClass() { P1 = 5, P2 = 8 }));
            Assert.AreEqual(15547530592771145767UL, HashObject.Hash64(new MyClass() { P1 = 3214, P2 = -98763524 }));
            Assert.AreEqual(3873731951615198575UL, HashObject.Hash64(new MyClass2() { P1 = 3214, P2 = -98763524, P3 = -8451 }));
            Assert.AreEqual(430739925859802484UL, HashObject.Hash64(new MyClass2() { P1 = 3214, P2 = -98763524, P3 = 563432167869876L }));
            Assert.AreEqual(16186487976295417620UL, HashObject.Hash64(new MyClass3() { P1 = 3214, P3 = 563432167869876L, P4 = "Hello World!" }));
            Assert.AreEqual(5800321734187549122UL, HashObject.Hash64(new MyClass3() { P1 = 3214, P3 = 563432167869876L, P4 = "hello world!" }));
            Assert.AreEqual(5005897306396511328UL, HashObject.Hash64(new MyClass4()));
        }
        [TestMethod]
        public void Perf1000()
        {
            var o = new MyClass3() { P1 = 3214, P3 = 563432167869876L, P4 = "hello world!" };
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(5800321734187549122UL, HashObject.Hash64(o));
            }
        }
        [TestMethod]
        public void Perf10000()
        {
            var o = new MyClass3() { P1 = 3214, P3 = 563432167869876L, P4 = "hello world!" };
            for (int i = 0; i < 10000; i++)
            {
                Assert.AreEqual(5800321734187549122UL, HashObject.Hash64(o));
            }
        }
        [TestMethod]
        public void Perf10000_threadsafety()
        {
            var passed = Enumerable.Range(0, 10000)
                .AsParallel()
                .Select(i => new MyClass3() { P1 = 3214, P3 = 563432167869876L, P4 = "hello world!" })
                .All(o => HashObject.Hash64(o) == 5800321734187549122UL)
                ;

            Assert.IsTrue(passed);
        }
        [TestMethod]
        public void Perf1000_nocache()
        {
            var o = new MyClass3() { P1 = 3214, P3 = 563432167869876L, P4 = "hello world!" };
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(5800321734187549122UL, HashObject.GetHashNoCache(o));
            }
        }
        [TestMethod]
        public void Perf10000_nocache()
        {
            var o = new MyClass3() { P1 = 3214, P3 = 563432167869876L, P4 = "hello world!" };
            for (int i = 0; i < 10000; i++)
            {
                Assert.AreEqual(5800321734187549122UL, HashObject.GetHashNoCache(o));
            }
        }
    }
}
