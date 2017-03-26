using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using SSGui;

namespace SpreadsheetGUITests
{
    [TestClass]
    public class SpreadsheetGUITests
    {
        /// <summary>
        /// Tests changing cell selections and set cell contents
        /// </summary>
        [TestMethod]
        public void Test1()
        {
            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub);

            stub.SelectionChanged(0, 0);
            Assert.AreEqual("A1", stub.CurrentCell);

            stub.FireContentBoxEvent("10");
            Assert.AreEqual("10", stub.ValueBox);

            stub.SelectionChanged(0, 1);
            Assert.AreEqual("A2", stub.CurrentCell);

            stub.FireContentBoxEvent("=A1");
            Assert.AreEqual("10", stub.ValueBox);
            Assert.AreEqual("=A1", stub.ContentBox);

            stub.SelectionChanged(4, 5);
            stub.SelectionChanged(0, 1);

            Assert.AreEqual("=A1", stub.ContentBox);
        }

        /// <summary>
        /// Tests Opening a saved Spreadsheet
        /// </summary>
        [TestMethod]
        public void TestOpen()
        {
            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub);

            stub.FireOpen("../../Test1.ss");
            Assert.IsTrue(stub.CalledOpenNewWithFile);
        }

        /// <summary>
        /// Tests Opening a new window
        /// </summary>
        [TestMethod]
        public void TestNew()
        {
            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub);

            stub.FireNewWindow();
            Assert.IsTrue(stub.CalledOpenNew);
        }

        /// <summary>
        /// Tests opening the help menu
        /// </summary>
        [TestMethod]
        public void TestHelp()
        {
            string msg = "To select a cell simply click on it.\n\nTo edit the contents of a cell select the cell and then use the contents box located in the"
                + " top right to edit the contents.\n\nUse the calculate button to commit your changes and calculate the cell's value.\n\nThe value of the selected"
                + " cell will be displayed in the value box which is located to the left of the content box.\n\nTo open or save a Spreadsheet file, open a new window,"
                + " or close the window use the \"file\" menu.";

            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub);

            stub.FireHowTo();
            Assert.AreEqual(msg, stub.Message);
        }

        /// <summary>
        /// Tests closing a window without changing anything
        /// </summary>
        [TestMethod]
        public void TestClose()
        {
            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub);

            stub.FireClose();
            Assert.IsFalse(stub.CalledSaveBeforeClose);
        }

        /// <summary>
        /// Tests closing a window after changing the Spreadsheet
        /// </summary>
        [TestMethod]
        public void TestCloseSave()
        {
            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub);

            stub.FireContentBoxEvent("10");

            stub.FireClose();
            Assert.IsTrue(stub.CalledSaveBeforeClose);
        }

        /// <summary>
        /// Tests trying to Save an empty spreadsheet
        /// </summary>
        [TestMethod]
        public void TestNoSave()
        {
            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub);

            stub.FireSave("../../SaveTest.ss");
            Assert.AreEqual("The file was not saved becasue the file has not been changed since the last save or since being opened.", stub.Message);
        }

        /// <summary>
        /// Test saving a spreadsheet
        /// </summary>
        [TestMethod]
        public void TestSave()
        {
            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub);

            stub.FireContentBoxEvent("");

            stub.FireSave("../../SaveTest.ss");
            Assert.AreEqual("Save Successful.", stub.Message);
        }

        /// <summary>
        /// Test an invalid formula
        /// </summary>
        [TestMethod]
        public void TestInvalidFormula()
        {
            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub);

            stub.FireContentBoxEvent("10");

            stub.FireContentBoxEvent("=F3++F4");
            Assert.AreEqual("Invalid Formula.", stub.Message);
            Assert.AreEqual("10", stub.ValueBox);
            Assert.AreEqual("=F3++F4", stub.ContentBox);

            stub.SelectionChanged(0, 1);
            stub.SelectionChanged(0, 0);
            Assert.AreEqual("10", stub.ContentBox);
            Assert.AreEqual("10", stub.ValueBox);
        }

        /// <summary>
        /// Test an Circular Dependency
        /// </summary>
        [TestMethod]
        public void TestCircularDependency()
        {
            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub);

            stub.FireContentBoxEvent("10");

            stub.FireContentBoxEvent("=A1");
            Assert.AreEqual("Circular Dependencies are not Allowed.", stub.Message);
            Assert.AreEqual("10", stub.ValueBox);
            Assert.AreEqual("=A1", stub.ContentBox);

            stub.SelectionChanged(0, 1);
            stub.SelectionChanged(0, 0);
            Assert.AreEqual("10", stub.ContentBox);
            Assert.AreEqual("10", stub.ValueBox);
        }

        /// <summary>
        /// Test a controller built from a saved spreadsheet.
        /// </summary>
        [TestMethod]
        public void TestController2()
        {
            SSWindowStub stub = new SSWindowStub();
            Controller controller = new Controller(stub, "../../Test1.ss");

            stub.SelectionChanged(0, 0);
            Assert.AreEqual("hello", stub.ContentBox);
            Assert.AreEqual("hello", stub.ValueBox);

            stub.SelectionChanged(1, 0);
            Assert.AreEqual("1", stub.ValueBox);
            Assert.AreEqual("1", stub.ContentBox);

            stub.SelectionChanged(1, 1);
            Assert.AreEqual("0", stub.ValueBox);
            Assert.AreEqual("=A2", stub.ContentBox);

            stub.SelectionChanged(0, 1);
            Assert.AreEqual("0", stub.ValueBox);
            Assert.AreEqual("0", stub.ContentBox);
        }
    }
}
