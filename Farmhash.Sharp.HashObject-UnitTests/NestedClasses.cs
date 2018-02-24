using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Farmhash.Sharp;
using System.Collections.Generic;

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
        class MyClassWithList
        {
            public IList<int> P1 { get; set; } = new List<int> { 1, 27, 35, 41, 56 };
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

        [TestMethod]
        public void VerifyNestedClassWithList()
        {
            Assert.AreEqual(HashObject.Hash64(new { P1 = 1, P27 = 27, P35 = 35, P41 = 41, P56 = 56 }), HashObject.Hash64(new MyClassWithList()));
            Assert.AreEqual(HashObject.Hash64(new { P1 = 1, P27 = 27, P35 = 35, P41 = 41, P56 = 56 }), HashObject.Hash64(new { P1 = new List<int> { 1, 27 }, P2 = new { P3 = new List<int> { 35 }, P41 = 41, P56 = 56 } }));
            Assert.AreEqual(HashObject.Hash64(new { P1 = 1, P27 = 27, P35 = 35, P41 = 41, P56 = 56 }), HashObject.Hash64(new { P1 = new List<MyClassWithList>() { new MyClassWithList() } }));
            Assert.AreEqual(HashObject.Hash64(new { P1 = new MyClassWithList().P1, P2 = new MyClassWithList().P1, P3 = new MyClassWithList().P1 }), HashObject.Hash64(new { P1 = new List<MyClassWithList>() { new MyClassWithList(), new MyClassWithList(), new MyClassWithList() } }));
        }
    }
}
