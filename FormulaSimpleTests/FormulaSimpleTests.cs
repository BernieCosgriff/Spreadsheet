// Written by Joe Zachary for CS 3500, January 2016.
// Repaired error in Evaluate5.  Added TestMethod Attribute
//    for Evaluate4 and Evaluate5 - JLZ January 25, 2016
// Corrected comment for Evaluate3 - JLZ January 29, 2016

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using System.Collections.Generic;

namespace FormulaTestCases
{
    /// <summary>
    /// These test cases are in no sense comprehensive!  They are intended to show you how
    /// client code can make use of the Formula class, and to show you how to create your
    /// own (which we strongly recommend).  To run them, pull down the Test menu and do
    /// Run > All Tests.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// This tests that a syntactically incorrect parameter to Formula results
        /// in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct1()
        {
            Formula f = new Formula("_");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct2()
        {
            Formula f = new Formula("2++3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct3()
        {
            Formula f = new Formula("2 3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct4()
        {
            Formula f = new Formula("-1");
        }

        /// <summary>
        /// Invalid character.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct5()
        {
            Formula f = new Formula("$x");
        }

        /// <summary>
        /// Invalid character.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct6()
        {
            Formula f = new Formula("(x+y)/(z-#)");
        }

        /// <summary>
        /// Invalid character.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct7()
        {
            Formula f = new Formula("(x+y)/(6^5)");
        }

        /// <summary>
        /// Not enough closing parenthesis.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct8()
        {
            Formula f = new Formula("(((2))");
        }

        /// <summary>
        /// Too many closing parethesis
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct9()
        {
            Formula f = new Formula("(2))");
        }

        /// <summary>
        /// Too many closing parethesis
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct10()
        {
            Formula f = new Formula("      ");
        }

        /// <summary>
        /// Correct syntax.
        /// </summary>
        [TestMethod]
        public void Evaluate8()
        {
            Formula f = new Formula("x");
            Assert.AreEqual(f.Evaluate(x => 19), 19.0, 1e-6);
        }

        /// <summary>
        /// Invalid Symbol
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidSymbol()
        {
            Formula f = new Formula("&");
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1()
        {
            Formula f = new Formula("2+3");
            Assert.AreEqual(f.Evaluate(v => 0), 5.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate2()
        {
            Formula f = new Formula("x5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// UndefinedVariableException (meaning that no variables have
        /// values).  The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate3()
        {
            Formula f = new Formula("x + y");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below.  We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate4()
        {
            Formula f = new Formula("x + y");
            Assert.AreEqual(f.Evaluate(Lookup4), 10.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate5 ()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// Tests order of operations without parenthesis.
        /// </summary>
        [TestMethod]
        public void Evaluate6()
        {
            Formula f = new Formula("6/3*2");
            Assert.AreEqual(f.Evaluate(Lookup4), 4.0, 1e-6);
        }

        /// <summary>
        /// Test scientific notation double.
        /// </summary>
        [TestMethod]
        public void Evaluate7()
        {
            Formula f = new Formula("5e9/10+70");
            Assert.AreEqual(f.Evaluate(null), 500000070.0, 1e-6);
        }

        /// <summary>
        /// Test scientific notation double.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void DivisionByZero()
        {
            Formula f = new Formula("5e9/10+70/0");
            Assert.AreEqual(f.Evaluate(Lookup4), 10);
        }

        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(String v)
        {
            switch (v)
            {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                default: throw new UndefinedVariableException(v);
            }
        }

        /// <summary>
        /// Test scientific notation double.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void UndefinedVariable()
        {
            Formula f = new Formula("5e9/10+70+r");
            Assert.AreEqual(f.Evaluate(Lookup4), 10);
        }

        /// <summary>
        /// Creates a formula using the three parameter constructor.
        /// </summary>
        [TestMethod]
        public void Construct3Test()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0",  Caps, CapsCheck);
        }

        /// <summary>
        /// Make sure the default 0-arg constructor
        /// behaves as expected.
        /// </summary>
        [TestMethod]
        public void NoArgConstruct()
        {
            Formula f = new Formula();
            Assert.AreEqual(0, f.Evaluate(s => 10));
            Assert.IsTrue(String.Equals("0", f.ToString()));
        }

        /// <summary>
        /// Should throw an exception if validator is null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void NullValidator()
        {
            Formula f = new Formula("0", s => s, null);
        }

        /// <summary>
        /// Should throw an exception if normalizer is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullNormalizer()
        {
            Formula f = new Formula("0", null, s => true);
        }

        /// <summary>
        /// Make sure GetVariables is empty with the no arg constructor
        /// </summary>
        [TestMethod]
        public void EmptyGetVariables()
        {
            Formula f = new Formula();
            Assert.AreEqual(0, f.GetVariables().Count);
        }

        /// <summary>
        /// Test GetVariables
        /// </summary>
        [TestMethod]
        public void GetVariables1()
        {
            ISet<string> set;
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            Assert.AreEqual(3, (set = f.GetVariables()).Count);
            Assert.IsTrue(set.Contains("x"));
            Assert.IsTrue(set.Contains("y"));
            Assert.IsTrue(set.Contains("z"));
        }

        /// <summary>
        /// Test normalized GetVariables
        /// </summary>
        [TestMethod]
        public void GetVariables2()
        {
            ISet<string> set;
            Formula f = new Formula("(x + y) * (z / x) * 1.0", Caps, CapsCheck);
            Assert.AreEqual(3, (set = f.GetVariables()).Count);
            Assert.IsTrue(set.Contains("X"));
            Assert.IsTrue(set.Contains("Y"));
            Assert.IsTrue(set.Contains("Z"));
            Console.WriteLine(f.ToString());
            Assert.IsTrue(f.ToString().Equals("(X + Y) * (Z / X) * 1.0"));
        }
        /// <summary>
        /// Test a formula that does not pass the validator
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ValidatorFalse()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0", s => s, CapsCheck);
        }

        /// <summary>
        /// Test a normalizer that changes variables to invalid tokens
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadNormalizer()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0", s => "%", s => true);
        }

        /// <summary>
        /// Test a null formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NullFormula()
        {
            Formula f = new Formula(null);
        }

        /// <summary>
        /// Converts a string to all upper case.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Caps(string str)
        {
            return str.ToUpper();
        }

        /// <summary>
        /// Checks if a string is all upper case
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool CapsCheck(string str)
        {
            foreach(char c in str)
            {
                if(!Char.IsUpper(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
