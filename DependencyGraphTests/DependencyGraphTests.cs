using System;
using Dependencies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DependencyGraphTests
{
    [TestClass]
    public class DependencyGraphTests
    {
        /// <summary>
        /// Test AddDependency, HasDependents, and HasDependees
        /// </summary>
        [TestMethod]
        public void AddDependency1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            Assert.AreEqual(1, dg.Size);
            Assert.IsTrue(dg.HasDependents("a"));
            Assert.IsTrue(dg.HasDependees("b"));
        }

        /// <summary>
        /// Test AddDependency, HasDependents, and HasDependees
        /// </summary>
        [TestMethod]
        public void AddDependency2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("b", "c");
            Assert.AreEqual(3, dg.Size);
            Assert.IsTrue(dg.HasDependents("a"));
            Assert.IsTrue(dg.HasDependents("b"));
            Assert.IsTrue(dg.HasDependees("b"));
            Assert.IsTrue(dg.HasDependees("c"));
        }

        /// <summary>
        /// Test GetDependents
        /// </summary>
        [TestMethod]
        public void GetDependents()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("b", "c");
            dg.AddDependency("c", "d");
            Assert.AreEqual(4, dg.Size);
            List<string> depends = new List<string>(dg.GetDependents("a"));
            Assert.AreEqual(2, depends.Count);
            Assert.IsTrue(depends.Contains("c"));
            Assert.IsTrue(depends.Contains("b"));
        }

        /// <summary>
        /// Test GetDependents
        /// </summary>
        [TestMethod]
        public void GetDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("b", "c");
            dg.AddDependency("c", "d");
            Assert.AreEqual(4, dg.Size);
            List<string> depends = new List<string>(dg.GetDependees("d"));
            Assert.AreEqual(1, depends.Count);
            Assert.IsTrue(depends.Contains("c"));
        }

        /// <summary>
        /// Test Remove Dependency.
        /// </summary>
        [TestMethod]
        public void RemoveDependency()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("b", "c");
            dg.AddDependency("c", "d");
            dg.AddDependency("c", "e");
            dg.AddDependency("d", "f");
            dg.AddDependency("e", "f");
            Assert.AreEqual(7, dg.Size);
            dg.RemoveDependency("a", "c");
            Assert.AreEqual(6, dg.Size);
            List<string> depends = new List<string>(dg.GetDependents("a"));
            Assert.AreEqual(1, depends.Count);
            Assert.IsTrue(depends.Contains("b"));
        }

        /// <summary>
        /// Test HasDependents on an empty DG.
        /// </summary>
        [TestMethod]
        public void HasDependentsEmpty()
        {
            DependencyGraph dg = new DependencyGraph();
            Assert.IsFalse(dg.HasDependees("a"));
            Assert.AreEqual(0, dg.Size);
        }

        /// <summary>
        /// Test HasDependees on an empty DG.
        /// </summary>
        [TestMethod]
        public void HasDependeesEmpty()
        {
            DependencyGraph dg = new DependencyGraph();
            Assert.IsFalse(dg.HasDependents("a"));
            Assert.AreEqual(0, dg.Size);
        }

        /// <summary>
        /// Test GetDependents on an empty DG.
        /// </summary>
        [TestMethod]
        public void GetDependentsEmpty()
        {
            DependencyGraph dg = new DependencyGraph();
            List<string> list = new List<string>(dg.GetDependents("A"));
            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// Test GetDependees on an empty DG.
        /// </summary>
        [TestMethod]
        public void GetDependeesEmpty()
        {
            DependencyGraph dg = new DependencyGraph();
            List<string> list = new List<string>(dg.GetDependees("A"));
            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// Test ReplaceDependents.
        /// </summary>
        [TestMethod]
        public void ReplaceDependents()
        {
            DependencyGraph dg = new DependencyGraph();
            List<string> list = new List<String>{ "a", "b", "c"};
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("b", "c");
            dg.AddDependency("c", "d");
            dg.AddDependency("c", "e");
            dg.AddDependency("d", "f");
            dg.AddDependency("e", "f");
            Assert.AreEqual(7, dg.Size);
            dg.ReplaceDependents("c", list);
            Assert.AreEqual(8, dg.Size);
            List<string> num = new List<string>(dg.GetDependents("c"));

            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(num.Contains("a"));
            Assert.IsTrue(num.Contains("b"));
            Assert.IsTrue(num.Contains("c"));

        }

        /// <summary>
        /// Test Replace Dependees.
        /// </summary>
        [TestMethod]
        public void ReplaceDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            List<string> list = new List<String> { "a", "b", "c" };
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("b", "c");
            dg.AddDependency("c", "d");
            dg.AddDependency("c", "e");
            dg.AddDependency("d", "f");
            dg.AddDependency("e", "f");
            Assert.AreEqual(7, dg.Size);
            dg.ReplaceDependees("c", list);
            Assert.AreEqual(8, dg.Size);
            List<string> num = new List<string>(dg.GetDependees("c"));

            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(num.Contains("a"));
            Assert.IsTrue(num.Contains("b"));
            Assert.IsTrue(num.Contains("c"));
        }
        
        /// <summary>
        /// Test Replace Dependents on an empty DG.
        /// </summary>
        [TestMethod]
        public void ReplaceDependentsEmpty()
        {
            DependencyGraph dg = new DependencyGraph();
            List<string> list = new List<String> { "a", "b", "c" };
            dg.ReplaceDependents("c", list);
            List<string> strs = new List<string>(dg.GetDependents("c"));
            Assert.IsTrue(strs.Contains("a"));
            Assert.IsTrue(strs.Contains("b"));
            Assert.IsTrue(strs.Contains("c"));
        }

        /// <summary>
        /// Test ReplaceDependees on an empty DG. 
        /// </summary>
        [TestMethod]
        public void ReplaceDependeesEmpty()
        {
            DependencyGraph dg = new DependencyGraph();
            List<string> list = new List<String> { "a", "b", "c" };
            dg.ReplaceDependees("c", list);
            List<string> strs = new List<string>(dg.GetDependees("c"));
            Assert.IsTrue(strs.Contains("a"));
            Assert.IsTrue(strs.Contains("b"));
            Assert.IsTrue(strs.Contains("c"));
        }

        /// <summary>
        /// Make sure duplicates are not added
        /// </summary>
        [TestMethod]
        public void AddDependencyDuplicate()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "b");
            Assert.AreEqual(1, dg.Size);
        }

    }
}
