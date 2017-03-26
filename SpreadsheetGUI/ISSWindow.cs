using System;
using SSGui;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS;

namespace SpreadsheetGUI
{
    public interface ISSWindow
    {
        /// <summary>
        /// The last save location
        /// </summary>
        string Filename { set; get; }

        /// <summary>
        /// Displays the cell's value
        /// </summary>
        string ValueBox { set; get; }

        /// <summary>
        /// Displays the cell's contents
        /// </summary>
        string ContentBox { set; get; }

        /// <summary>
        /// Displays the Current Cell's name
        /// </summary>
        string CurrentCell { set; get; }

        /// <summary>
        /// Displays messages to the UI
        /// </summary>
        string Message { set; }

        /// <summary>
        /// The SpreadsheetPanel being used by the window
        /// </summary>
        SpreadsheetPanel SPanel { get; }

        //Events
        event Action CloseEvent;

        event Action HelpEvent;

        event Action<string> SaveEvent;

        event Action<string> OpenEvent;

        event Action NewWindowEvent;

        event Action CellEvent;

        event Action ContentBoxEvent;

        /// <summary>
        /// Opens new window
        /// </summary>
        void OpenNew();

        /// <summary>
        /// Opens a file in a new window
        /// </summary>
        void OpenNew(string filename);

        /// <summary>
        /// Saves the spreadsheet before closing
        /// </summary>
        void SaveBeforeClose();
    }
}
