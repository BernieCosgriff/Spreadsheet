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
            List<string> list = new List<String> { "a", "b", "c" };
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

        /// <summary>
        /// Test 1-arg constructor
        /// </summary>
        [TestMethod]
        public void ConstructFromDG()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("a", "d");
            Assert.AreEqual(3, dg.Size);

            DependencyGraph dg2 = new DependencyGraph(dg);
            Assert.IsTrue(dg2.HasDependees("b"));
            Assert.IsTrue(dg2.HasDependees("c"));
            Assert.IsTrue(dg2.HasDependees("d"));
            Assert.IsTrue(dg2.HasDependents("a"));
            Assert.AreEqual(3, dg2.Size);
            Assert.IsTrue(dg.HasDependees("c"));

            dg.RemoveDependency("a", "c");
            Assert.IsFalse(dg.HasDependees("c"));

            Assert.IsTrue(dg2.HasDependees("c"));
        }

        /// <summary>
        /// Test ReplaceDependees with 1-arg constructor
        /// </summary>
        [TestMethod]
        public void ReplaceDependees2()
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
            DependencyGraph dgg = new DependencyGraph(dg);
            Assert.AreEqual(8, dgg.Size);
            List<string> num = new List<string>(dgg.GetDependees("c"));

            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(num.Contains("a"));
            Assert.IsTrue(num.Contains("b"));
            Assert.IsTrue(num.Contains("c"));
        }

        /// <summary>
        /// Test ReplaceDependents with 1-arg constructor.
        /// </summary>
        [TestMethod]
        public void ReplaceDependents2()
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
            dg.ReplaceDependents("c", list);
            Assert.AreEqual(8, dg.Size);
            DependencyGraph dgg = new DependencyGraph(dg);
            Assert.AreEqual(8, dgg.Size);
            List<string> num = new List<string>(dgg.GetDependents("c"));

            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(num.Contains("a"));
            Assert.IsTrue(num.Contains("b"));
            Assert.IsTrue(num.Contains("c"));
        }

        /// <summary>
        /// Make sure we can use the 1-arg constructor
        /// when the dg is empty.
        /// </summary>
        [TestMethod]
        public void EmptyConstructor()
        {
            DependencyGraph dg = new DependencyGraph();
            DependencyGraph dg2 = new DependencyGraph(dg);
        }

        /// <summary>
        /// Test a null in constructor
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConstructor()
        {
            DependencyGraph dg = new DependencyGraph(null);
        }

        /// <summary>
        /// Test a null in AddDependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAdd1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency(null, "a");
        }

        /// <summary>
        /// Test a null in AddDependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAdd2()
        {
            DependencyGraph dg = new DependencyGraph();

            dg.AddDependency("b", null);
        }

        /// <summary>
        /// Test a null in GetDependees
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullGetDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.GetDependees(null);
        }

        /// <summary>
        /// Test a null in GetDependents
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullGetDependents()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.GetDependents(null);
        }

        /// <summary>
        /// Test a null in HasDependees
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullHasDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.HasDependees(null);
        }

        /// <summary>
        /// Test a null in HasDependents
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullHasDependents()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.HasDependents(null);
        }

        /// <summary>
        /// Test a null in RemoveDependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullRemove1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.RemoveDependency("s", null);
        }

        /// <summary>
        /// Test a null in RemoveDependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullRemove2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.RemoveDependency(null, "s");
        }

        /// <summary>
        /// Test a null in ReplaceDependees
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullReplaceDependees1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.ReplaceDependees(null, new HashSet<string>());
        }

        /// <summary>
        /// Test a null in ReplaceDependees
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullReplaceDependees2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.ReplaceDependees("s", null);
        }

        /// <summary>
        /// Test a null in ReplaceDependents
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullReplaceDependents1()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.ReplaceDependents(null, new HashSet<string>());
        }

        /// <summary>
        /// Test a null in ReplaceDependents
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullReplaceDependents2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.ReplaceDependents("s", null);
        }

        // Stress Test from PS3GradingTests but using the second constructor before checking correctness //
        /// <summary>
        ///Using lots of data with replacement
        ///</summary>
        [TestMethod()]
        public void StressTest15()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 800;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 2; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Replace a bunch of dependees
            for (int i = 0; i < SIZE; i += 2)
            {
                HashSet<string> newDees = new HashSet<String>();
                for (int j = 0; j < SIZE; j += 9)
                {
                    newDees.Add(letters[j]);
                }
                t.ReplaceDependees(letters[i], newDees);

                foreach (string s in dees[i])
                {
                    dents[s[0] - 'a'].Remove(letters[i]);
                }

                foreach (string s in newDees)
                {
                    dents[s[0] - 'a'].Add(letters[i]);
                }

                dees[i] = newDees;
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                DependencyGraph t2 = new DependencyGraph(t);
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t2.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t2.GetDependees(letters[i]))));
            }
        }
    }
}
