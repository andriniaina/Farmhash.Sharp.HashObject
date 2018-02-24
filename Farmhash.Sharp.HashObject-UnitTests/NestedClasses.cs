using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Farmhash.Sharp;

namespace Farmhash.Sharp.UnitTests
{
    [TestClass]
    public class NestedClasses
    {
        class MySubClass
        {
            public int SP1 { get; set; }
            public int SP2 { get; set; }
        }
        class MyClass
        {
            public int P1 { get; set; }
            public MySubClass P2 { get; set; }
        }

        [TestMethod]
        public void VerifyNestedClassesBasic()
        {
            var h2 = HashObject.Hash64(new { P1 = 123, P2 = new { SP1 = 654 } });
            Assert.AreEqual(7817622667888063637UL, h2);
        }

        [TestMethod]
        public void VerifyNestedClassesSimilarMembers()
        {
            var h1 = HashObject.Hash64(new { P1 = 123, P2 = 654 });
            var h2 = HashObject.Hash64(new { P1 = 123, P2 = new { SP1 = 654 } });

            Assert.AreEqual(h1, h2);
        }
    }
}
