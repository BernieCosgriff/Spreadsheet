using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace SpreadsheetTests
{
    /// <summary>
    /// Class to test the Spreadsheet class
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {

        private Regex validName = new Regex("^[A-Z]+[1-9][0-9]*$");

        /// <summary>
        /// Tests SetContentsOfCell with invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsInvalidName()
        {
            Spreadsheet s = new Spreadsheet(validName);
            s.SetContentsOfCell("A", "a");
        }

        /// <summary>
        /// Tests SetContentsOfCell with invalid formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetContentsInvalidFormula()
        {
            Spreadsheet s = new Spreadsheet(validName);
            s.SetContentsOfCell("A1", "=a+b");
        }

        /// <summary>
        /// Tests SetContentsOfCell for an invalid Formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetContentsInvalid()
        {
            Spreadsheet s = new Spreadsheet(validName);
            s.SetContentsOfCell("A1", "=a+b");
        }

        /// <summary>
        /// Tests SetContentsOfCell for a null contents
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContentsNull()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", null);
        }

        /// <summary>
        /// Tells whether a cells value is a FormulaError
        /// </summary>
        /// <param name="name"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool isFormulaError(string name, Spreadsheet s)
        {
            return s.GetCellValue(name) is FormulaError;
        }

        /// <summary>
        /// Tests SetContentsOfCell with only one cell filled
        /// </summary>
        [TestMethod]
        public void SetContentsBasic()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("A1", "=a+b").Count);
        }

        /// <summary>
        /// Tests SetContentsOfCell with only one cell filled
        /// </summary>
        [TestMethod]
        public void SetContentsUndefinedVarFormulaError()
        {
            Spreadsheet s = new Spreadsheet(validName);
            s.SetContentsOfCell("A1", "=B1");
            Assert.IsTrue(isFormulaError("A1", s));
        }

        /// <summary>
        /// Tests SetContentsOfCell with only one cell filled
        /// </summary>
        [TestMethod]
        public void SetContentsDivBy0FormulaError()
        {
            Spreadsheet s = new Spreadsheet(validName);
            s.SetContentsOfCell("B1", "1");
            s.SetContentsOfCell("C1", "0");
            s.SetContentsOfCell("A1", "=B1/C1");
            Assert.IsTrue(isFormulaError("A1", s));
        }

        /// <summary>
        /// Test save and load a valid xml file
        /// </summary>
        [TestMethod]
        public void XMLTestValid()
        {
            Spreadsheet s = new Spreadsheet(validName);
            s.SetContentsOfCell("A1", "=B1");
            s.SetContentsOfCell("B1", "10");
            s.SetContentsOfCell("B2", "=A3");
            s.SetContentsOfCell("A2", "hello");
            Assert.IsTrue(s.Changed);
            s.Save(new StreamWriter("../../valid.xml"));

            Spreadsheet s2 = new Spreadsheet(new StreamReader("../../valid.xml"));
            HashSet<string> set = new HashSet<string>(s2.GetNamesOfAllNonemptyCells());
            Assert.IsTrue(set.Contains("A1"));
            Assert.IsTrue(set.Contains("B1"));
            Assert.IsTrue(set.Contains("A2"));
        }

        /// <summary>
        /// Test load an xml file with a duplicate cell
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLTestDuplicate()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../ssdup.xml"));
        }

        /// <summary>
        /// Test load an xml file with an invalid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLTestBadName()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../badname.xml"));
        }

        /// <summary>
        /// Test load an xml file with an invalid formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLTestBadForm()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../badform.xml"));
        }

        /// <summary>
        /// Test load an xml file with a Circular Dependency
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLTestCircularDependency()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../circdepend.xml"));
        }

        /// <summary>
        /// Test load an invalid xml file.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLTestInvalid()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../invalid.xml"));
        }

        /// <summary>
        /// Test load an invalid xml file.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void XMLTestInvalid2()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../invalid2.xml"));
        }

        /// <summary>
        /// Test replacing a cell with a formula with a Circular Dependency
        /// </summary>
        [TestMethod]
        public void ReplaceWithCircularFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a", "i");
            s.SetContentsOfCell("b", "=a");
            try
            {
                s.SetContentsOfCell("a", "=b");
            }
            catch (CircularException)
            {
                Assert.AreEqual("i", s.GetCellContents("a"));
                Assert.AreEqual("i", s.GetCellValue("a"));
            }
        }

        /// <summary>
        /// Test replacing a cell with an empty string
        /// </summary>
        [TestMethod]
        public void MakeCellEmpty()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a", "=z");
            Assert.IsTrue(isFormulaError("a", s));
            s.SetContentsOfCell("a", "");
            Assert.IsFalse(isFormulaError("a", s));
            Assert.AreEqual("", s.GetCellValue("a"));
            Assert.AreEqual("", s.GetCellContents("a"));
        }

        /// <summary>
        /// Test replacing a cell with a double
        /// </summary>
        [TestMethod]
        public void ReplaceWithDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a", "=z");
            Assert.IsTrue(isFormulaError("a", s));
            s.SetContentsOfCell("a", "10");
            Assert.IsFalse(isFormulaError("a", s));
            Assert.AreEqual(10.0, (double) s.GetCellValue("a"));
            Assert.AreEqual(10.0, s.GetCellContents("a"));
        }

        /// <summary>
        /// Test getting cell value with null name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValueNullName()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellValue(null);
        }

        /// <summary>
        /// Test getting cell value with an invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValueInvalidName()
        {
            Spreadsheet s = new Spreadsheet(validName);
            s.GetCellValue("b");
        }

        /// <summary>
        /// Test getting cell value of an empty cell
        /// </summary>
        [TestMethod]
        public void GetEmptyCellValue()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellValue("b"));
        }

        /// <summary>
        /// Tests SetCellContents with a formula and a double
        /// </summary>
        [TestMethod]
        public void SetFormulaAndDouble()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            ISet<string> set = s.SetCellContents("a1", 10);
            s.SetCellContents("a2", new Formula("a1 + 2"));
            ISet<string> set2 = s.SetCellContents("a1", 10);
            Assert.IsTrue(set.Contains("A1"));
            Assert.IsTrue(set2.Contains("A1"));
            Assert.IsTrue(set2.Contains("A2"));
            Assert.AreEqual(10.0, s.GetCellContents("A1"));
            Assert.AreEqual("A1 + 2", s.GetCellContents("a2").ToString().ToUpper());
        }

        /// <summary>
        /// Tests that SetCellContents double throws an InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameDouble()
        {
            TestableSpreadsheet s = new TestableSpreadsheet(validName);
            s.SetCellContents("A", 10);
        }

        /// <summary>
        /// Tests that SetCellContents text throws an InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameText()
        {
            TestableSpreadsheet s = new TestableSpreadsheet(validName);
            s.SetCellContents("A", "text");
        }

        /// <summary>
        /// Tests that SetCellContents formula throws an InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName()
        {
            TestableSpreadsheet s = new TestableSpreadsheet(validName); ;
            s.SetCellContents("A", new Formula());
        }

        /// <summary>
        /// Tests GetNonEmptyCells
        /// </summary>
        [TestMethod]
        public void GetNonEmptyCells()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.SetCellContents("a1", 10);
            s.SetCellContents("c3", 11);
            IEnumerator<string> en = s.GetNamesOfAllNonemptyCells().GetEnumerator();
            en.MoveNext();
            Assert.AreEqual("A1", en.Current);
            en.MoveNext();
            Assert.AreEqual("C3", en.Current);
        }

        /// <summary>
        /// Tests setting a cell with something already in it
        /// </summary>
        [TestMethod]
        public void ReplaceContents()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.SetCellContents("a1", 10);
            s.SetCellContents("c3", 11);
            Assert.AreEqual(10.0, s.GetCellContents("a1"));
            s.SetCellContents("a1", new Formula("X1e2"));
            Assert.AreEqual("X1e2", s.GetCellContents("a1").ToString());
            s.SetCellContents("c3", "overwrite");
            Assert.AreEqual("overwrite", s.GetCellContents("c3").ToString());
        }

        /// <summary>
        /// Tests SetCellContents with text
        /// </summary>
        [TestMethod]
        public void SetCellText()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.SetCellContents("A1294039", "hello");
            s.SetCellContents("C334", "world");
            List<string> list = new List<string>(s.GetNamesOfAllNonemptyCells());
            Assert.IsTrue(list.Contains("A1294039"));
            Assert.AreEqual("hello", s.GetCellContents("A1294039"));
            Assert.AreEqual("world", s.GetCellContents("C334"));
            Assert.IsTrue(list.Contains("C334"));
        }

        /// <summary>
        /// Tests that SetCellContents throws a CircularException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularExcpetion()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.SetCellContents("a1", new Formula("c3 + 1"));
            s.SetCellContents("c3", new Formula("a1 + 1"));
        }

        /// <summary>
        /// Tests SetCellContents throws NullArgument with double
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellNullDouble()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.SetCellContents(null, 10);
        }

        /// <summary>
        /// Tests SetCellContents throws NullArgument with text
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellNullText()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.SetCellContents(null, "hello");
        }

        /// <summary>
        /// Tests SetCellContents throws NullArgument with Formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellNullForumla()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.SetCellContents(null, new Formula());
        }

        /// <summary>
        /// Test getting an empty cell's contents
        /// </summary>
        [TestMethod]
        public void GetEmptyCellsContents()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            Assert.AreEqual("", s.GetCellContents("a1"));
        }

        /// <summary>
        /// Tests SetCellContents throws NullArgument
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellNull()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.SetCellContents("a1", null);
        }

        /// <summary>
        /// Tests GetCellContents throws NullArgument
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellNull1()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.GetCellContents(null);
        }

        /// <summary>
        /// Test GetDirectDependents
        /// </summary>
        [TestMethod]
        public void GetDirectDependents()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.SetCellContents("a1", new Formula("a2 + 3"));
            s.SetCellContents("a2", new Formula("b1 * 2"));
            s.SetCellContents("b3", new Formula("a1 + 2"));
            List<string> list = new List<string>(s.SetCellContents("B1", new Formula("10")));
            Assert.IsTrue(list.Contains("B1"));
            Assert.IsTrue(list.Contains("A2"));
            Assert.IsTrue(list.Contains("A1"));
            Assert.IsTrue(list.Contains("B3"));
        }

        /// <summary>
        /// Test an invalid name in SetCellContents with a double
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameSetDouble()
        {
            TestableSpreadsheet s = new TestableSpreadsheet(validName);
            s.SetCellContents("a", 10);
        }

        /// <summary>
        /// Test an invalid name in SetCellContents with text
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameSetText()
        {
            TestableSpreadsheet s = new TestableSpreadsheet(validName);
            s.SetCellContents("a", "contents");
        }

        /// <summary>
        /// Test an invalid name in SetCellContents with a Formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameSetFormula()
        {
            TestableSpreadsheet s = new TestableSpreadsheet(validName);
            s.SetCellContents("a", new Formula());
        }

        /// <summary>
        /// Replace a Formula with a different formula
        /// </summary>
        [TestMethod]
        public void ReplaceFormula()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            ISet<string> set = s.SetCellContents("a1", new Formula("a2 + b1"));
            s.SetCellContents("A1", new Formula("A3"));
            ISet<string> set2 = s.SetCellContents("a3", 5);

            Assert.IsTrue(set2.Contains("A1"));
            Assert.IsTrue(set2.Contains("A3"));
            Assert.AreEqual(2, set2.Count);
        }

        /// <summary>
        /// Makes sure that Uppercase and Lowercase
        /// cell names reference the same cell.
        /// </summary>
        [TestMethod]
        public void UpperVsLower()
        {
            TestableSpreadsheet s = new TestableSpreadsheet();
            s.SetCellContents("a1", 10.0);
            Assert.AreEqual(s.GetCellContents("a1"), s.GetCellContents("A1"));
            Assert.AreEqual(s.GetCellContents("A1"), 10.0);
            s.SetCellContents("A1", 5.0);
            Assert.AreEqual(s.GetCellContents("a1"), s.GetCellContents("A1"));
            Assert.AreEqual(s.GetCellContents("a1"), 5.0);
        }

        /// <summary>
        /// Class that allows old test Cases to work with new implementation.
        /// </summary>
        public class TestableSpreadsheet : Spreadsheet

        {
            public TestableSpreadsheet() : base()
            {
            }

            public TestableSpreadsheet(Regex name) : base(name)
            {
            }

            public new ISet<string> SetCellContents(string name, string text)

            {

                return base.SetCellContents(name, text);

            }

            public new ISet<string> SetCellContents(string name, double number)

            {

                return base.SetCellContents(name, number);

            }

            public new ISet<string> SetCellContents(string name, Formula formula)

            {

                return base.SetCellContents(name, formula);

            }
        }
    }
}
