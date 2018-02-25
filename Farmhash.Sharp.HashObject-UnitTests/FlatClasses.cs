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
            public decimal P15 { get; set; } = 21587.2489010165841984M;
        }

        public enum MyIntEnum
        {
            _, a, b, c, d, e, f, g, h
        }
        public enum MyLongEnum : long
        {
            _, a, b, c, d, e, f, g, h
        }
        public enum MyULongEnum : ulong
        {
            _, a, b, c, d, e, f, g, h
        }
        public enum MyByteEnum : byte
        {
            _, a, b, c, d, e, f, g, h
        }

        [TestMethod]
        public void Verify()
        {
            Assert.AreEqual(11160318154034397263UL, HashObject.Hash64(new { P1 = (string)null }));
            Assert.AreEqual(11160318154034397263UL, HashObject.Hash64(new { P1 = (MyClass4)null }));

            Assert.AreEqual(5161771770385946686UL, HashObject.Hash64(new { P1 = true }));
            Assert.AreEqual(6831060280857550628UL, HashObject.Hash64(new { P1 = (byte)5 }));
            Assert.AreEqual(6635462465573064060UL, HashObject.Hash64(new { P1 = (sbyte)-5 }));
            Assert.AreEqual(17632198867199723397UL, HashObject.Hash64(new { P1 = (ushort)300 }));
            Assert.AreEqual(14828412535119066885UL, HashObject.Hash64(new { P1 = (short)125 }));
            Assert.AreEqual(15889518867509351608UL, HashObject.Hash64(new { P1 = -2154654 }));
            Assert.AreEqual(13504660158532017398UL, HashObject.Hash64(new { P1 = 79846u }));
            Assert.AreEqual(17184136070396980929UL, HashObject.Hash64(new { P1 = 6543685498321218174ul }));
            Assert.AreEqual(13597913617216260015UL, HashObject.Hash64(new { P1 = -216549867321456L }));
            Assert.AreEqual(10964267548211513180UL, HashObject.Hash64(new { P1 = 'a' }));
            Assert.AreEqual(8088892088076493022UL, HashObject.Hash64(new { P1 = 321654.32165497D }));
            Assert.AreEqual(10869235659710969429UL, HashObject.Hash64(new { P1 = 147852369F }));
            Assert.AreEqual(12485759631462869615UL, HashObject.Hash64(new { P1 = 213.02545989002144M }));
            Assert.AreEqual(5454119810501579372UL, HashObject.Hash64(new { P1 = new DateTime(2018, 1, 5, 12, 32, 45, 321, DateTimeKind.Local) }));
            Assert.AreEqual(HashObject.Hash64(new { P1 = MyIntEnum.c }), HashObject.Hash64(new { P1 = 3L }));
            Assert.AreEqual(HashObject.Hash64(new { P1 = MyLongEnum.c }), HashObject.Hash64(new { P1 = 3L }));
            Assert.AreEqual(HashObject.Hash64(new { P1 = MyULongEnum.c }), HashObject.Hash64(new { P1 = 3L }));
            Assert.AreEqual(HashObject.Hash64(new { P1 = MyByteEnum.c }), HashObject.Hash64(new { P1 = 3L }));

            Assert.AreEqual(14924216523195090866UL, HashObject.Hash64(new MyClass() { P1 = 5, P2 = 8 }));
            Assert.AreEqual(15547530592771145767UL, HashObject.Hash64(new MyClass() { P1 = 3214, P2 = -98763524 }));
            Assert.AreEqual(3873731951615198575UL, HashObject.Hash64(new MyClass2() { P1 = 3214, P2 = -98763524, P3 = -8451 }));
            Assert.AreEqual(430739925859802484UL, HashObject.Hash64(new MyClass2() { P1 = 3214, P2 = -98763524, P3 = 563432167869876L }));
            Assert.AreEqual(16186487976295417620UL, HashObject.Hash64(new MyClass3() { P1 = 3214, P3 = 563432167869876L, P4 = "Hello World!" }));
            Assert.AreEqual(5800321734187549122UL, HashObject.Hash64(new MyClass3() { P1 = 3214, P3 = 563432167869876L, P4 = "hello world!" }));
            Assert.AreEqual(14034018172463916860UL, HashObject.Hash64(new MyClass4()));
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
                Assert.AreEqual(5800321734187549122UL, HashObject.Hash64_NoCache_forBenchmarks(o));
            }
        }
        [TestMethod]
        public void Perf10000_nocache()
        {
            var o = new MyClass3() { P1 = 3214, P3 = 563432167869876L, P4 = "hello world!" };
            for (int i = 0; i < 10000; i++)
            {
                Assert.AreEqual(5800321734187549122UL, HashObject.Hash64_NoCache_forBenchmarks(o));
            }
        }
    }
}
