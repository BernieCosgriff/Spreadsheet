using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetGUI;
using SSGui;

namespace SpreadsheetGUITests
{
    class SSWindowStub : ISSWindow
    {

        public string Filename { set; get; }

        public SSWindowStub()
        {
            ssPanel = new SpreadsheetPanel();
            currentCell = "A1";
        }

        private SpreadsheetPanel ssPanel;

        /// <summary>
        /// Tells whether OpenNew was called
        /// </summary>
        public bool CalledOpenNew { get; private set; }

        /// <summary>
        /// Tells whether CloseBeforeSave was called
        /// </summary>
        public bool CalledSaveBeforeClose { get; private set; }

        /// <summary>
        /// Tells whether OpenNew was called
        /// </summary>
        public bool CalledOpenNewWithFile { get; private set; }

        /// <summary>
        /// Contents of the current cell
        /// </summary>
        private string contentBox;

        /// <summary>
        /// Value of the current cell
        /// </summary>
        private string valueBox;

        /// <summary>
        /// Value of the current cell
        /// </summary>
        private string currentCell;


        /// <summary>
        /// Displays the contents of the current cell
        /// </summary>
        public string ContentBox { get { return contentBox; } set { contentBox = value; } }

        /// <summary>
        /// Displays the value of the current cell
        /// </summary>
        public string ValueBox { get { return valueBox; } set { valueBox = value; } }

        /// <summary>
        /// Displays the Current Cell's name
        /// </summary>
        public string CurrentCell { set { currentCell = value; } get { return currentCell; } }

        /// <summary>
        /// Retrieves messages passed from controller
        /// </summary>
        public string Message { set; get; }

        public SpreadsheetPanel SPanel { get { return ssPanel; } }

        //Events
        public event Action CellEvent;
        public event Action CloseEvent;
        public event Action ContentBoxEvent;
        public event Action NewWindowEvent;
        public event Action<string> OpenEvent;
        public event Action<string> SaveEvent;
        public event Action HelpEvent;

        /// <summary>
        /// Sets CalledOpenNew to true
        /// </summary>
        public void OpenNew()
        {
            CalledOpenNew = true;
        }

        /// <summary>
        /// Fires the CellEvent
        /// </summary>
        /// <param name="ss"></param>
        public void SelectionChanged(int col, int row)
        {
            ssPanel.SetSelection(col, row);
            if (CellEvent != null)
            {
                CellEvent();
            }
        }

        /// <summary>
        /// Fires SaveEvent
        /// </summary>
        /// <param name="filename"></param>
        public void FireSave(string filename)
        {
            SaveEvent(filename);
        }

        /// <summary>
        /// Fires NewWindowEvent
        /// </summary>
        public void FireNewWindow()
        {
            if (NewWindowEvent != null)
            {
                NewWindowEvent();
            }
        }

        /// <summary>
        /// Fires CloseEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FireClose()
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
        }

        /// <summary>
        /// Fires HelpEvent
        /// </summary>
        public void FireHowTo()
        {
            if(HelpEvent != null)
            {
                HelpEvent();
            }
        }

        /// <summary>
        /// Fires ContextBoxEvent
        /// </summary>
        /// <param name="ssPanel"></param>
        public void FireContentBoxEvent(string content)
        {
            contentBox = content;
            if (ContentBoxEvent != null)
            {
                ContentBoxEvent();
            }
        }

        public void OpenNew(string filename)
        {
            CalledOpenNewWithFile = true;
        }

        public void SaveBeforeClose()
        {
            CalledSaveBeforeClose = true;
        }

        public void FireOpen(string filename)
        {
            if(OpenEvent != null)
            {
                OpenEvent(filename);
            }
        }
    }
}
